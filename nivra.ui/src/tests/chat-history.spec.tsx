import { act, render, screen, waitFor, within } from "@testing-library/react";
import { vi } from "vitest";
import ChatHistory from "../components/ChatHistory";
import { createChatConnection } from "../classes/chat-connection";

type FakeConnection = {
    on: ReturnType<typeof vi.fn>;
    onreconnected: ReturnType<typeof vi.fn>;
    start: ReturnType<typeof vi.fn>;
    stop: ReturnType<typeof vi.fn>;
};

function latestConnection(): FakeConnection {
    const results = vi.mocked(createChatConnection).mock.results;
    return results[results.length - 1].value as unknown as FakeConnection;
}

function receiveMessageHandler() {
    const call = latestConnection().on.mock.calls.find((call) => call[0] === "ReceiveMessage");
    if (!call) {
        throw new Error("ChatHistory did not subscribe to ReceiveMessage");
    }
    return call[1] as (message: unknown) => void;
}

const MESSAGES_URL = "http://localhost:5207/api/Chat/Messages";

function mockJsonOnce(body: unknown, ok = true) {
    vi.mocked(fetch).mockResolvedValueOnce({
        ok,
        status: ok ? 200 : 500,
        json: async () => body,
        text: async () => JSON.stringify(body)
    } as Response);
}

const threeMessages = [
    { id: 1, dateT: "2026-10-01T09:01:00Z", message: "oldest message", userId: 1, username: "alice" },
    { id: 2, dateT: "2026-10-01T09:02:00Z", message: "middle message", userId: 1, username: "alice" },
    { id: 3, dateT: "2026-10-01T09:03:00Z", message: "newest message", userId: 2, username: "bob" }
];

describe("ChatHistory", () => {
    beforeEach(() => {
        vi.stubGlobal("fetch", vi.fn());
    });

    afterEach(() => {
        vi.unstubAllGlobals();
        vi.restoreAllMocks();
    });

    it("loads the history from the chat API", async () => {
        mockJsonOnce([]);

        render(<ChatHistory />);

        await waitFor(() => expect(fetch).toHaveBeenCalledTimes(1));
        expect(vi.mocked(fetch).mock.calls[0][0]).toBe(MESSAGES_URL);
    });

    it("renders every message oldest first", async () => {
        mockJsonOnce(threeMessages);

        render(<ChatHistory />);

        await screen.findByText("newest message");
        const items = within(screen.getByRole("list")).getAllByRole("listitem");
        expect(items.map((item) => item.textContent)).toEqual([
            expect.stringContaining("oldest message"),
            expect.stringContaining("middle message"),
            expect.stringContaining("newest message")
        ]);
    });

    it("shows each message time as machine-readable markup", async () => {
        mockJsonOnce(threeMessages);

        render(<ChatHistory />);

        const times = await screen.findAllByRole("time");
        expect(times).toHaveLength(3);
        expect(times[0]).toHaveAttribute("dateTime", "2026-10-01T09:01:00Z");
    });

    it("shows a friendly message when the chat is empty", async () => {
        mockJsonOnce([]);

        render(<ChatHistory />);

        expect(await screen.findByText(/no messages yet/i)).toBeInTheDocument();
        expect(screen.queryByRole("list")).not.toBeInTheDocument();
    });

    it("shows the username of each sender", async () => {
        mockJsonOnce(threeMessages);

        render(<ChatHistory />);

        await screen.findByText("newest message");
        expect(screen.getAllByText("alice")).toHaveLength(2);
        expect(screen.getByText("bob")).toBeInTheDocument();
    });

    it("shows an error when the history cannot be loaded", async () => {
        mockJsonOnce([], false);

        render(<ChatHistory />);

        expect(await screen.findByRole("alert")).toHaveTextContent(
            "Could not load chat history"
        );
    });

    it("shows an error when the network fails", async () => {
        vi.mocked(fetch).mockRejectedValueOnce(new Error("connection refused"));

        render(<ChatHistory />);

        expect(await screen.findByRole("alert")).toHaveTextContent(
            "Could not load chat history"
        );
    });

    it("connects to the chat hub and listens for new messages", async () => {
        mockJsonOnce(threeMessages);

        render(<ChatHistory />);

        await screen.findByText("newest message");
        expect(latestConnection().start).toHaveBeenCalledTimes(1);
        expect(receiveMessageHandler()).toBeTypeOf("function");
    });

    it("appends a message pushed by the hub without reloading the history", async () => {
        mockJsonOnce(threeMessages);
        render(<ChatHistory />);
        await screen.findByText("newest message");

        act(() => {
            receiveMessageHandler()({
                id: 4,
                dateT: "2026-10-01T09:04:00Z",
                message: "live message",
                userId: 2,
                username: "bob",
                equippedSkin: ""
            });
        });

        expect(screen.getByText("live message")).toBeInTheDocument();
        expect(fetch).toHaveBeenCalledTimes(1);
    });

    it("ignores a pushed message that is already shown", async () => {
        mockJsonOnce(threeMessages);
        render(<ChatHistory />);
        await screen.findByText("newest message");

        act(() => {
            receiveMessageHandler()(threeMessages[2]);
        });

        expect(screen.getAllByText("newest message")).toHaveLength(1);
    });

    it("stops the hub connection when the chat is closed", async () => {
        mockJsonOnce([]);
        const { unmount } = render(<ChatHistory />);
        await screen.findByText(/no messages yet/i);

        unmount();

        expect(latestConnection().stop).toHaveBeenCalledTimes(1);
    });
});