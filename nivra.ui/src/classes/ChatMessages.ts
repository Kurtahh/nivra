export const CHAT_POLL_INTERVAL_MS = 3000;

// The backend timestamps a message when its background worker stores it, so the
// stored time can be slightly before or after the moment the browser sent it.
const PENDING_MATCH_WINDOW_MS = 60_000;

export interface ChatMessage {
    id: number;
    dateT: string;
    message: string;
    userId: number;
    username: string;
    equippedSkin: string;
}

export interface PendingChatMessage {
    id: number;
    message: string;
    userId: number;
    sentAt: number;
}

function parseChatDate(dateT: string): number {
    // The API may serialize a UTC timestamp without a trailing "Z" when the
    // database column has no timezone. Treat such values as UTC.
    const hasTimeZone = /(?:Z|[+-]\d{2}:?\d{2})$/i.test(dateT);
    return new Date(hasTimeZone ? dateT : `${dateT}Z`).getTime();
}

export function isPendingMessageDelivered(
    pending: PendingChatMessage,
    messages: ChatMessage[]
): boolean {
    return messages.some((chatMessage) =>
        chatMessage.userId === pending.userId
        && chatMessage.message === pending.message
        && parseChatDate(chatMessage.dateT) >= pending.sentAt - PENDING_MATCH_WINDOW_MS
    );
}
