import '@testing-library/jest-dom/vitest'
import { vi } from 'vitest'

vi.mock('./classes/chat-connection', () => ({
    CHAT_HUB_URL: 'http://localhost:5207/hubs/chat',
    RECEIVE_MESSAGE_EVENT: 'ReceiveMessage',
    createChatConnection: vi.fn(() => ({
        on: vi.fn(),
        onreconnected: vi.fn(),
        start: vi.fn().mockResolvedValue(undefined),
        stop: vi.fn().mockResolvedValue(undefined)
    }))
}))