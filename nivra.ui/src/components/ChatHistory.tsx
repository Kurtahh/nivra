import { useEffect, useState } from "react";

const messagesUrl = "http://localhost:5207/api/Chat/Messages";

interface ChatMessage {
    id: number;
    dateT: string;
    message: string;
    userId: number;
}




export default function ChatHistory() {
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [errorMsg, setErrorMsg] = useState("");
    const [loading, setLoading] = useState(true);

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
    }, []);

    if (loading) {
        return <p className="chat-loading">Loading chat history...</p>;
    }

    if (errorMsg !== "") {
        return <p className="chat-error" role="alert">{errorMsg}</p>;
    }

    if (messages.length === 0) {
        return <p className="chat-empty">No messages yet</p>;
    }

    return (
        <ul className="chat-history">
            {messages.map((chatMessage) => (
                <li className="chat-message" key={chatMessage.id}>
                    <text >{}</text>
                    <span className="chat-text">{chatMessage.message}</span>
                    <time className="chat-time" dateTime={chatMessage.dateT}>
                        {new Date(chatMessage.dateT).toLocaleTimeString([], {
                            hour: "2-digit",
                            minute: "2-digit"
                        })}
                    </time>
                </li>
            ))}
        </ul>
    );
}
