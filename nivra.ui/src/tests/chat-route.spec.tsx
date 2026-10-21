import { render, screen } from "@testing-library/react";
import { vi } from "vitest";
import App from "../App";

describe("chat route", () => {
    beforeEach(() => {
        vi.stubGlobal(
            "fetch",
            vi.fn().mockResolvedValue({
                ok: true,
                status: 200,
                json: async () => []
            } as Response)
        );
        window.history.pushState({}, "", "/chat");
    });

    afterEach(() => {
        vi.unstubAllGlobals();
        vi.restoreAllMocks();
        window.history.pushState({}, "", "/");
    });

    it("renders the chat page at /chat", async () => {
        render(<App />);

        expect(
            await screen.findByPlaceholderText(/type a message/i)
        ).toBeInTheDocument();
        expect(await screen.findByText(/no messages yet/i)).toBeInTheDocument();
    });

    it("links to the chat page from the navigation", () => {
        render(<App />);

        expect(screen.getByRole("link", { name: /chat/i })).toHaveAttribute(
            "href",
            "/chat"
        );
    });
});
