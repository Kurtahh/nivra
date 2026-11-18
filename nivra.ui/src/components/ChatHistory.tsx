import { useEffect, useRef, useState } from "react";
import {
    isPendingMessageDelivered,
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
    const [reconnectCount, setReconnectCount] = useState(0);
    const onMessagesLoadedRef = useRef(onMessagesLoaded);

    useEffect(() => {
        onMessagesLoadedRef.current = onMessagesLoaded;
    }, [onMessagesLoaded]);

    useEffect(() => {
        onMessagesLoadedRef.current?.(messages);
    }, [messages]);

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
                    setMessages(history);
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

    useEffect(() => {
        let cancelled = false;
        const connection = createChatConnection();

        connection.on(RECEIVE_MESSAGE_EVENT, (message: ChatMessage) => {
            if (cancelled) {
                return;
            }
            setMessages((previous) =>
                previous.some((existing) => existing.id === message.id)
                    ? previous
                    : [...previous, message]
            );
        });

        connection.onreconnected(() => {
            if (!cancelled) {
                setReconnectCount((count) => count + 1);
            }
        });

        connection.start().catch(() => {
            if (!cancelled) {
                setErrorMsg("Live chat is unavailable. Reload the page to see new messages.");
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
            <ul className="chat-history">
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