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

export function parseChatDate(dateT: string): number {
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

// Combines two message lists without duplicates (a message with the same id
// is taken from `incoming`) and returns them oldest first, like the API does.
export function mergeChatMessages(
    current: ChatMessage[],
    incoming: ChatMessage[]
): ChatMessage[] {
    const byId = new Map<number, ChatMessage>();
    for (const message of current) {
        byId.set(message.id, message);
    }
    for (const message of incoming) {
        byId.set(message.id, message);
    }
    return [...byId.values()].sort((a, b) =>
        parseChatDate(a.dateT) - parseChatDate(b.dateT) || a.id - b.id
    );
}