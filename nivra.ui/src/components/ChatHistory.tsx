import { useEffect, useRef, useState } from "react";
import {
    isPendingMessageDelivered,
    mergeChatMessages,
    type ChatMessage,
    type PendingChatMessage
} from "../classes/ChatMessages.ts";
import { Session } from "../classes/Session.ts";
import { skinIcon } from "../classes/ShopApi.ts";
import { createChatConnection, RECEIVE_MESSAGE_EVENT } from "../classes/chat-connection.ts";
import ProfilePopup from "./ProfilePopup.tsx";

const messagesUrl = "http://localhost:5207/api/Chat/Messages";

type Props = {
    pendingMessages?: PendingChatMessage[];
    onMessagesLoaded?: (messages: ChatMessage[]) => void;
    refreshToken?: number;
};

export default function ChatHistory({
    pendingMessages = [],
    onMessagesLoaded,
    refreshToken = 0
}: Props) {
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [errorMsg, setErrorMsg] = useState("");
    const [loading, setLoading] = useState(true);
    const [profileUsername, setProfileUsername] = useState<string | null>(null);
    const [liveErrorMsg, setLiveErrorMsg] = useState("");
    const [reconnectCount, setReconnectCount] = useState(0);
    const listRef = useRef<HTMLUListElement>(null);
    const stickToBottom = useRef(true);
    const onMessagesLoadedRef = useRef(onMessagesLoaded);

    useEffect(() => {
        onMessagesLoadedRef.current = onMessagesLoaded;
    }, [onMessagesLoaded]);

    // Lets the parent clear "Sending..." messages once they show up,
    // whether they came from the history request or from the live hub.
    useEffect(() => {
        onMessagesLoadedRef.current?.(messages);
    }, [messages]);

    // Load the full history on mount, after sending a message, and after
    // the live connection reconnects (messages may have been missed meanwhile).
    useEffect(() => {
        let cancelled = false;

        async function loadHistory() {
            try {
                const response = await fetch(messagesUrl);
                if (!response.ok) {
                    throw new Error(`Chat history request failed: ${response.status}`);
                }
                const history: ChatMessage[] = await response.json();
                if (!cancelled) {
                    // Merge instead of replacing: a message pushed by the hub while
                    // this request was in flight may be missing from the response.
                    setMessages((previous) => mergeChatMessages(previous, history));
                    setErrorMsg("");
                }
            } catch {
                if (!cancelled) {
                    setErrorMsg("Could not load chat history");
                }
            } finally {
                if (!cancelled) {
                    setLoading(false);
                }
            }
        }

        loadHistory();

        return () => {
            cancelled = true;
        };
    }, [refreshToken, reconnectCount]);

    // Live updates: the server pushes every newly saved message.
    useEffect(() => {
        let cancelled = false;
        const connection = createChatConnection();

        connection.on(RECEIVE_MESSAGE_EVENT, (message: ChatMessage) => {
            if (cancelled) {
                return;
            }
            setMessages((previous) => mergeChatMessages(previous, [message]));
        });

        connection.onreconnected(() => {
            if (!cancelled) {
                setLiveErrorMsg("");
                setReconnectCount((count) => count + 1);
            }
        });

        connection.start()
            .then(() => {
                if (!cancelled) {
                    setLiveErrorMsg("");
                }
            })
            .catch((error: unknown) => {
                if (!cancelled) {
                    console.error("Could not connect to the chat hub", error);
                    setLiveErrorMsg("Live chat is unavailable. Reload the page to see new messages.");
                }
            });

        return () => {
            cancelled = true;
            connection.stop();
        };
    }, []);

    const visiblePendingMessages = pendingMessages.filter(
        (pendingMessage) => !isPendingMessageDelivered(pendingMessage, messages)
    );
    const hasMessages = messages.length > 0 || visiblePendingMessages.length > 0;

    // Keep the newest message in view, unless the user has scrolled up to read older ones.
    useEffect(() => {
        const list = listRef.current;
        if (list && stickToBottom.current) {
            list.scrollTop = list.scrollHeight;
        }
    }, [messages.length, visiblePendingMessages.length, loading]);

    function handleScroll() {
        const list = listRef.current;
        if (list) {
            stickToBottom.current = list.scrollHeight - list.scrollTop - list.clientHeight < 80;
        }
    }

    if (loading) {
        return <p className="chat-loading">Loading chat history...</p>;
    }

    if (!hasMessages) {
        if (errorMsg !== "") {
            return <p className="chat-error" role="alert">{errorMsg}</p>;
        }

        return <p className="chat-empty">No messages yet</p>;
    }

    return (
        <>
            {errorMsg !== "" && (
                <p className="chat-error" role="alert">{errorMsg}</p>
            )}
            {liveErrorMsg !== "" && (
                <p className="chat-error" role="status">{liveErrorMsg}</p>
            )}
            <ul className="chat-history" ref={listRef} onScroll={handleScroll}>
                {messages.map((chatMessage) => (
                    <li className="chat-message" key={chatMessage.id}>
                        <button
                            type="button"
                            className="chat-username chat-username-button"
                            onClick={() => setProfileUsername(chatMessage.username)}
                            aria-label={`Open ${chatMessage.username}'s profile`}
                        >
                            <span aria-hidden="true">{skinIcon(chatMessage.equippedSkin)} </span>
                            {chatMessage.username}
                        </button>
                        <span className="chat-text">{chatMessage.message}</span>
                        <time className="chat-time" dateTime={chatMessage.dateT}>
                            {new Date(chatMessage.dateT).toLocaleTimeString([], {
                                hour: "2-digit",
                                minute: "2-digit"
                            })}
                        </time>
                    </li>
                ))}
                {visiblePendingMessages.map((pendingMessage) => (
                    <li
                        className="chat-message chat-message-pending"
                        key={`pending-${pendingMessage.id}`}
                    >
                        <button
                            type="button"
                            className="chat-username chat-username-button"
                            onClick={() => setProfileUsername(String(Session.instance.getUsername()))}
                            aria-label="Open your profile"
                        >
                            {String(Session.instance.getUsername())}
                        </button>
                        <span className="chat-text">{pendingMessage.message}</span>
                        <span className="chat-pending" role="status">Sending...</span>
                    </li>
                ))}
            </ul>
            {profileUsername !== null && (
                <ProfilePopup
                    username={profileUsername}
                    onClose={() => setProfileUsername(null)}
                />
            )}
        </>
    );
}