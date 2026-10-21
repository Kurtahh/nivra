import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { vi } from "vitest";
import Chat from "../components/Chat";

const MESSAGES_URL = "http://localhost:5207/api/Chat/Messages";

const oldMessage = {
    id: 1,
    dateT: "2026-10-01T09:01:00Z",
    message: "old message",
    userId: 1
};

const newMessage = {
    id: 2,
    dateT: "2026-10-01T09:05:00Z",
    message: "new message",
    userId: 1
};

function jsonResponse(body: unknown, ok = true) {
    return {
        ok,
        status: ok ? 200 : 500,
        json: async () => body,
        text: async () => JSON.stringify(body)
    } as Response;
}

describe("Chat page", () => {
    beforeEach(() => {
        vi.stubGlobal("fetch", vi.fn());
    });

    afterEach(() => {
        vi.unstubAllGlobals();
        vi.restoreAllMocks();
    });

    it("shows the history and the input together", async () => {
        vi.mocked(fetch).mockResolvedValue(jsonResponse([oldMessage]));

        render(<Chat />);

        expect(await screen.findByText("old message")).toBeInTheDocument();
        expect(screen.getByPlaceholderText(/type a message/i)).toBeInTheDocument();
        expect(screen.getByRole("button", { name: /send/i })).toBeInTheDocument();
    });

    it("reloads the history after a message is sent", async () => {
        let historyRequests = 0;
        vi.mocked(fetch).mockImplementation(async (input) => {
            const url = String(input);
            if (url === MESSAGES_URL) {
                historyRequests += 1;
                return historyRequests === 1
                    ? jsonResponse([oldMessage])
                    : jsonResponse([oldMessage, newMessage]);
            }
            return jsonResponse("Message sent successfully!");
        });

        render(<Chat />);
        await screen.findByText("old message");

        fireEvent.change(screen.getByPlaceholderText(/type a message/i), {
            target: { value: "new message" }
        });
        fireEvent.click(screen.getByRole("button", { name: /send/i }));

        expect(await screen.findByText("new message")).toBeInTheDocument();
        await waitFor(() => expect(historyRequests).toBe(2));
    });

    it("keeps the history when a send fails", async () => {
        let historyRequests = 0;
        vi.mocked(fetch).mockImplementation(async (input) => {
            const url = String(input);
            if (url === MESSAGES_URL) {
                historyRequests += 1;
                return jsonResponse([oldMessage]);
            }
            return jsonResponse("Message is above 200!", false);
        });

        render(<Chat />);
        await screen.findByText("old message");

        fireEvent.change(screen.getByPlaceholderText(/type a message/i), {
            target: { value: "new message" }
        });
        fireEvent.click(screen.getByRole("button", { name: /send/i }));

        expect(await screen.findByRole("alert")).toHaveTextContent(
            "Message is above 200!"
        );
        expect(screen.getByText("old message")).toBeInTheDocument();
        expect(historyRequests).toBe(1);
    });
});
