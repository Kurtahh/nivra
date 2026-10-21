import { render, screen } from "@testing-library/react";
import { vi } from "vitest";
import { Leaderboard } from "../components/Leaderboard";

const entries = [
    { userId: 1, username: "vincent", totalSteps: 12000, rank: 1 },
    { userId: 2, username: "ada", totalSteps: 9000, rank: 2 }
];

describe("Leaderboard", () => {
    beforeEach(() => {
        vi.stubGlobal(
            "fetch",
            vi.fn().mockResolvedValue({
                ok: true,
                status: 200,
                json: async () => entries
            } as Response)
        );
    });

    afterEach(() => {
        vi.unstubAllGlobals();
        vi.restoreAllMocks();
    });

    it("styles itself with classes instead of inline styles", async () => {
        const { container } = render(<Leaderboard />);

        await screen.findByText("vincent");

        expect(screen.getByRole("table")).toHaveClass("leaderboard-table");
        expect(container.querySelectorAll("[style]")).toHaveLength(0);
    });

    it("shows the rank, user and steps for every entry", async () => {
        render(<Leaderboard />);

        await screen.findByText("vincent");

        const rows = screen.getAllByRole("row").slice(1);
        expect(rows).toHaveLength(2);
        expect(rows[0]).toHaveTextContent("#1");
        expect(rows[0]).toHaveTextContent("vincent");
        expect(rows[0]).toHaveTextContent("12,000");
    });
});
