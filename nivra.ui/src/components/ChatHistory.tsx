import { useEffect, useRef, useState } from "react";
import {
    CHAT_POLL_INTERVAL_MS,
    isPendingMessageDelivered,
    type ChatMessage,
    type PendingChatMessage
} from "../classes/ChatMessages.ts";
import { Session } from "../classes/Session.ts";

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
    const onMessagesLoadedRef = useRef(onMessagesLoaded);

    useEffect(() => {
        onMessagesLoadedRef.current = onMessagesLoaded;
    }, [onMessagesLoaded]);

    useEffect(() => {
        let cancelled = false;
        let requestInFlight = false;

        async function loadHistory() {
            if (requestInFlight) {
                return;
            }

            requestInFlight = true;
            try {
                const response = await fetch(messagesUrl);
                if (!response.ok) {
                    throw new Error(`Chat history request failed: ${response.status}`);
                }
                const history: ChatMessage[] = await response.json();
                if (!cancelled) {
                    setMessages(history);
                    setErrorMsg("");
                    onMessagesLoadedRef.current?.(history);
                }
            } catch {
                if (!cancelled) {
                    setErrorMsg("Could not load chat history");
                }
            } finally {
                requestInFlight = false;
                if (!cancelled) {
                    setLoading(false);
                }
            }
        }

        loadHistory();
        const intervalId = window.setInterval(loadHistory, CHAT_POLL_INTERVAL_MS);

        return () => {
            cancelled = true;
            window.clearInterval(intervalId);
        };
    }, [refreshToken]);

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
                        <span className="chat-username">{chatMessage.username}</span>
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
                        <span className="chat-username">{String(Session.instance.getUsername())}</span>
                        <span className="chat-text">{pendingMessage.message}</span>
                        <span className="chat-pending" role="status">Sending...</span>
                    </li>
                ))}
            </ul>
        </>
    );
}
