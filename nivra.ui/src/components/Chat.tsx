import ChatHistory from "./ChatHistory.tsx";
import ChatInput from "./ChatInput.tsx";
import { useCallback, useRef, useState } from "react";
import { Session } from "../classes/Session.ts";
import {
    isPendingMessageDelivered,
    type ChatMessage,
    type PendingChatMessage
} from "../classes/ChatMessages.ts";


export default function Chat() {
    const [pendingMessages, setPendingMessages] = useState<PendingChatMessage[]>([]);
    const [refreshToken, setRefreshToken] = useState(0);
    const nextPendingId = useRef(0);

    const handleMessageSent = useCallback((message: string) => {
        nextPendingId.current += 1;
        setPendingMessages((previous) => [
            ...previous,
            {
                id: nextPendingId.current,
                message,
                userId: Number(Session.instance.getId()),
                sentAt: Date.now()
            }
        ]);
        setRefreshToken((token) => token + 1);
    }, []);

    const handleMessagesLoaded = useCallback((messages: ChatMessage[]) => {
        setPendingMessages((previous) => {
            const remaining = previous.filter(
                (pendingMessage) => !isPendingMessageDelivered(pendingMessage, messages)
            );
            return remaining.length === previous.length ? previous : remaining;
        });
    }, []);

    return(
        <section className="chat-page">
            <h1 className="page-title">Chat</h1>
            <ChatHistory
                pendingMessages={pendingMessages}
                onMessagesLoaded={handleMessagesLoaded}
                refreshToken={refreshToken}
            ></ChatHistory>
            <ChatInput onMessageSent={handleMessageSent}></ChatInput>
        </section>
    );
}
