import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { vi } from "vitest";
import ChatInput from "../components/ChatInput";
import { Session } from "../classes/Session";

const SEND_MESSAGE_URL = "http://localhost:5207/api/Chat/SendMessage";

function typeMessage(text: string) {
    fireEvent.change(screen.getByPlaceholderText(/type a message/i), {
        target: { value: text }
    });
}

function clickSend() {
    fireEvent.click(screen.getByRole("button", { name: /send/i }));
}

function mockFetchOnce(response: Partial<Response>) {
    vi.mocked(fetch).mockResolvedValueOnce(response as Response);
}

describe("ChatInput", () => {
    beforeEach(() => {
        vi.stubGlobal("fetch", vi.fn());
    });

    afterEach(() => {
        vi.unstubAllGlobals();
        vi.restoreAllMocks();
    });

    it("renders a text input with placeholder text", () => {
        render(<ChatInput />);

        const input = screen.getByPlaceholderText(/type a message/i);
        expect(input).toBeInTheDocument();
        expect(input).toHaveAttribute("type", "text");
    });

    it("shows what the user types", () => {
        render(<ChatInput />);

        typeMessage("Hello my health-nuts");

        expect(screen.getByPlaceholderText(/type a message/i)).toHaveValue(
            "Hello my health-nuts"
        );
    });

    it("posts the message to the chat endpoint for the logged-in user", async () => {
        const getId = vi.spyOn(Session.instance, "getId").mockReturnValue(7);
        mockFetchOnce({
            ok: true,
            status: 200,
            text: async () => "Message sent successfully!"
        });

        render(<ChatInput />);
        typeMessage("Hello my health-nuts");
        clickSend();

        await waitFor(() => expect(fetch).toHaveBeenCalledTimes(1));

        const [url, options] = vi.mocked(fetch).mock.calls[0];
        expect(url).toBe(SEND_MESSAGE_URL);
        expect(options?.method).toBe("POST");
        expect(JSON.parse(options?.body as string)).toEqual({
            message: "Hello my health-nuts",
            userId: 7
        });
        expect(getId).toHaveBeenCalled();
    });

    it("clears the input after the message is sent", async () => {
        mockFetchOnce({
            ok: true,
            status: 200,
            text: async () => "Message sent successfully!"
        });

        render(<ChatInput />);
        typeMessage("Hello my health-nuts");
        clickSend();

        await waitFor(() =>
            expect(screen.getByPlaceholderText(/type a message/i)).toHaveValue("")
        );
    });

    it("shows the backend error and keeps the message when the server rejects it", async () => {
        mockFetchOnce({
            ok: false,
            status: 400,
            text: async () => "Message is above 200!"
        });

        render(<ChatInput />);
        typeMessage("Hello my health-nuts");
        clickSend();

        expect(await screen.findByRole("alert")).toHaveTextContent(
            "Message is above 200!"
        );
        expect(screen.getByPlaceholderText(/type a message/i)).toHaveValue(
            "Hello my health-nuts"
        );
    });

    it("shows a generic error and keeps the message when the network fails", async () => {
        vi.mocked(fetch).mockRejectedValueOnce(new Error("connection refused"));

        render(<ChatInput />);
        typeMessage("Hello my health-nuts");
        clickSend();

        expect(await screen.findByRole("alert")).toHaveTextContent(
            "Could not connect to the server. Message was not sent."
        );
        expect(screen.getByPlaceholderText(/type a message/i)).toHaveValue(
            "Hello my health-nuts"
        );
    });
});
