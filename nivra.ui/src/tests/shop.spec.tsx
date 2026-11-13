import { fireEvent, render, screen, within } from "@testing-library/react";
import { beforeEach, afterEach, describe, expect, it, vi } from "vitest";
import Shop from "../components/Shop";

vi.mock("../classes/Session", () => ({
    Session: {
        instance: {
            getId: () => 1,
            getUsername: () => "walker",
        },
    },
}));

type MockOrder = { id: number; name: string; price: number };

const skins = [
    { id: 1, name: "Explorer", price: 0 },
    { id: 2, name: "Sprout", price: 100 },
    { id: 3, name: "Cosmic", price: 2000 },
];

function jsonResponse(body: unknown, ok = true, status = 200) {
    return {
        ok,
        status,
        text: async () => (body === undefined ? "" : JSON.stringify(body)),
        json: async () => body,
    } as Response;
}

function mockShop(options: {
    balance: number;
    orders: MockOrder[];
    totalSteps: number;
    onCreateOrder?: (order: MockOrder) => void;
    onUpdateAccount?: (balance: number) => void;
}) {
    vi.mocked(fetch).mockImplementation(async (input, init) => {
        const url = String(input);
        const method = init?.method ?? "GET";
        const body = init?.body ? JSON.parse(String(init.body)) : undefined;

        if (url.includes("/api/Skin")) {
            return jsonResponse(skins);
        }

        if (url.includes("/api/Account") && method === "GET") {
            return jsonResponse([
                { id: 1, name: "walker", balance: options.balance },
            ]);
        }

        if (url.includes("/api/Account/1") && method === "PUT") {
            options.onUpdateAccount?.(body.balance);
            return jsonResponse({
                id: 1,
                name: "walker",
                balance: body.balance,
            });
        }

        if (url.includes("/api/Order") && method === "GET") {
            return jsonResponse(options.orders);
        }

        if (url.endsWith("/api/Order") && method === "POST") {
            const order: MockOrder = {
                id: 100 + options.orders.length,
                name: body.name,
                price: body.price,
            };
            options.onCreateOrder?.(order);
            return jsonResponse(order);
        }

        if (url.includes("/api/Leaderboard")) {
            return jsonResponse([
                {
                    userId: 1,
                    username: "walker",
                    totalSteps: options.totalSteps,
                    rank: 1,
                },
            ]);
        }

        throw new Error(`Unexpected request: ${method} ${url}`);
    });
}

describe("Shop", () => {
    beforeEach(() => {
        vi.stubGlobal("fetch", vi.fn());
        localStorage.clear();
    });

    afterEach(() => {
        vi.unstubAllGlobals();
        vi.restoreAllMocks();
    });

    it("shows the user's step balance and marks free avatars as owned", async () => {
        mockShop({ balance: 1000, orders: [], totalSteps: 1000 });

        render(<Shop />);

        expect(await screen.findByText("1,000 steps")).toBeInTheDocument();

        const explorer = screen.getByRole("heading", { name: "Explorer" }).closest("li")!;
        expect(within(explorer).getByText("Owned")).toBeInTheDocument();
        expect(within(explorer).getByRole("button", { name: "Use avatar" })).toBeEnabled();

        const cosmic = screen.getByRole("heading", { name: "Cosmic" }).closest("li")!;
        expect(within(cosmic).getByRole("button", { name: "Not enough steps" })).toBeDisabled();
    });

    it("deducts the skin price and records an order", async () => {
        const created: MockOrder[] = [];
        const balances: number[] = [];
        mockShop({
            balance: 1000,
            orders: [],
            totalSteps: 1000,
            onCreateOrder: (order) => created.push(order),
            onUpdateAccount: (balance) => balances.push(balance),
        });

        render(<Shop />);
        await screen.findByText("1,000 steps");

        const sprout = screen.getByRole("heading", { name: "Sprout" }).closest("li")!;
        fireEvent.click(within(sprout).getByRole("button", { name: "Buy" }));

        expect(await screen.findByText("900 steps")).toBeInTheDocument();
        expect(within(sprout).getByText("Owned")).toBeInTheDocument();
        expect(balances).toEqual([900]);
        expect(created).toEqual([
            { id: 100, name: "purchase:1:2", price: 100 },
        ]);
        expect(screen.getByText("100 spent · 1,000 earned")).toBeInTheDocument();
    });

    it("lets the user equip an owned avatar and keeps the purchase history", async () => {
        let equippedOrder: MockOrder | null = null;
        mockShop({
            balance: 900,
            orders: [{ id: 10, name: "purchase:1:2", price: 100 }],
            totalSteps: 1000,
            onCreateOrder: (order) => {
                equippedOrder = order;
            },
        });

        render(<Shop />);
        await screen.findByText("900 steps");

        const sprout = screen.getByRole("heading", { name: "Sprout" }).closest("li")!;
        fireEvent.click(within(sprout).getByRole("button", { name: "Use avatar" }));

        expect(await within(sprout).findByText("Equipped")).toBeInTheDocument();
        expect(equippedOrder).toEqual({ id: 101, name: "equip:1:2", price: 0 });
        expect(screen.getByText("Sprout", { selector: ".shop-profile-copy strong" })).toBeInTheDocument();
        expect(JSON.parse(localStorage.getItem("nivra:avatar:1")!)).toMatchObject({
            id: 2,
            name: "Sprout",
            icon: "🌱",
        });
    });
});
