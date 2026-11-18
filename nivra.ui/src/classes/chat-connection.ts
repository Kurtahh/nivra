import { HubConnectionBuilder, LogLevel, type HubConnection } from "@microsoft/signalr";

export const CHAT_HUB_URL = "http://localhost:5207/hubs/chat";

export const RECEIVE_MESSAGE_EVENT = "ReceiveMessage";

export function createChatConnection(): HubConnection {
    return new HubConnectionBuilder()
        .withUrl(CHAT_HUB_URL, { withCredentials: false })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Warning)
        .build();
}