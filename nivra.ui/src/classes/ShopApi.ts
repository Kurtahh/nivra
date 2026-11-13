export type Skin = {
    id: number;
    name: string;
    price: number;
};

export type Account = {
    id: number;
    name: string;
    balance: number;
};

export type Order = {
    id: number;
    name: string;
    price: number;
};

export type Purchase = {
    orderId: number;
    skinId: number;
    price: number;
};

export type ShopState = {
    account: Account;
    skins: Skin[];
    purchases: Purchase[];
    equipOrder: Order | null;
    totalSteps: number;
};

export type CachedAvatar = {
    id: number;
    name: string;
    icon: string;
};

type LeaderboardEntry = {
    userId: number;
    username: string;
    totalSteps: number;
    rank: number;
};

const API_BASE = "http://localhost:5207/api";
const PURCHASE_PREFIX = "purchase";
const EQUIP_PREFIX = "equip";
const AVATAR_STORAGE_PREFIX = "nivra:avatar";

export const SHOP_AVATAR_EVENT = "nivra:avatar-changed";

const JSON_HEADERS = { "Content-Type": "application/json" };

const SKIN_ICONS: Record<string, string> = {
    explorer: "🧭",
    sprout: "🌱",
    sunrise: "🌅",
    trailblazer: "🥾",
    mountain: "⛰️",
    rocket: "🚀",
    champion: "🏆",
    cosmic: "🌌",
};

export function skinIcon(name: string): string {
    return SKIN_ICONS[name.trim().toLowerCase()] ?? "🏃";
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
    const response = await fetch(`${API_BASE}${path}`, init);

    if (!response.ok) {
        let message = `Request failed (${response.status})`;
        try {
            const body = await response.text();
            if (body) {
                message = body;
            }
        } catch {
            // Keep the generic message when the body cannot be read.
        }
        throw new Error(message);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    const body = await response.text();
    return (body ? JSON.parse(body) : undefined) as T;
}

export function purchaseOrderName(userId: number, skinId: number): string {
    return `${PURCHASE_PREFIX}:${userId}:${skinId}`;
}

export function equipOrderName(userId: number, skinId: number): string {
    return `${EQUIP_PREFIX}:${userId}:${skinId}`;
}

function parseShopOrderName(name: string, prefix: string, userId: number): number | null {
    const parts = name.split(":");
    if (parts.length !== 3 || parts[0] !== prefix) {
        return null;
    }

    if (Number(parts[1]) !== userId) {
        return null;
    }

    const skinId = Number(parts[2]);
    return Number.isInteger(skinId) && skinId > 0 ? skinId : null;
}

export function parsePurchaseOrder(order: Order, userId: number): Purchase | null {
    const skinId = parseShopOrderName(order.name, PURCHASE_PREFIX, userId);
    if (skinId === null) {
        return null;
    }

    return { orderId: order.id, skinId, price: order.price };
}

export function parseEquipOrder(order: Order, userId: number): number | null {
    return parseShopOrderName(order.name, EQUIP_PREFIX, userId);
}

export function ownedSkinIds(state: ShopState): Set<number> {
    const owned = new Set<number>(
        state.skins.filter((skin) => skin.price <= 0).map((skin) => skin.id)
    );
    state.purchases.forEach((purchase) => owned.add(purchase.skinId));
    return owned;
}

export function spentSteps(purchases: Purchase[]): number {
    return purchases.reduce((total, purchase) => total + purchase.price, 0);
}

export function maxSteps(account: Account, purchases: Purchase[], totalSteps: number): number {
    const fromAccount = account.balance + spentSteps(purchases);
    return Math.max(fromAccount, totalSteps);
}

export function availableSteps(
    account: Account,
    purchases: Purchase[],
    totalSteps: number
): number {
    return Math.max(0, maxSteps(account, purchases, totalSteps) - spentSteps(purchases));
}

export async function fetchSkins(): Promise<Skin[]> {
    return request<Skin[]>("/Skin");
}

export async function fetchAccounts(): Promise<Account[]> {
    return request<Account[]>("/Account");
}

export async function fetchOrders(): Promise<Order[]> {
    return request<Order[]>("/Order");
}

export async function fetchTotalSteps(userId: number): Promise<number> {
    try {
        const entries = await request<LeaderboardEntry[]>(
            "/Leaderboard?period=AllTime&top=100000"
        );
        return entries.find((entry) => Number(entry.userId) === userId)?.totalSteps ?? 0;
    } catch {
        // The shop still works from the account balance when the leaderboard is down.
        return 0;
    }
}

function findAccount(accounts: Account[], username: string): Account | undefined {
    const exact = accounts.find((account) => account.name === username);
    if (exact) {
        return exact;
    }

    const lowerCaseName = username.toLowerCase();
    return accounts.find((account) => account.name.toLowerCase() === lowerCaseName);
}

export async function createAccount(name: string, balance: number): Promise<Account> {
    return request<Account>("/Account", {
        method: "POST",
        headers: JSON_HEADERS,
        body: JSON.stringify({ name, balance }),
    });
}

export async function updateAccountBalance(account: Account, balance: number): Promise<Account> {
    return request<Account>(`/Account/${account.id}`, {
        method: "PUT",
        headers: JSON_HEADERS,
        body: JSON.stringify({ name: account.name, balance }),
    });
}

export async function createOrder(name: string, price: number): Promise<Order> {
    return request<Order>("/Order", {
        method: "POST",
        headers: JSON_HEADERS,
        body: JSON.stringify({ name, price }),
    });
}

export async function updateOrder(order: Order, name: string): Promise<Order> {
    return request<Order>(`/Order/${order.id}`, {
        method: "PUT",
        headers: JSON_HEADERS,
        body: JSON.stringify({ name, price: order.price }),
    });
}

export async function deleteOrder(order: Order): Promise<void> {
    await request<void>(`/Order/${order.id}`, { method: "DELETE" });
}

const inFlightLoads = new Map<string, Promise<ShopState>>();

export function loadShop(userId: number, username: string): Promise<ShopState> {
    const key = `${userId}:${username}`;
    const existing = inFlightLoads.get(key);
    if (existing) {
        return existing;
    }

    const load = loadShopOnce(userId, username).finally(() => {
        inFlightLoads.delete(key);
    });
    inFlightLoads.set(key, load);
    return load;
}

async function loadShopOnce(userId: number, username: string): Promise<ShopState> {
    const [skins, accounts, orders, totalSteps] = await Promise.all([
        fetchSkins(),
        fetchAccounts(),
        fetchOrders(),
        fetchTotalSteps(userId),
    ]);

    const purchases = orders
        .map((order) => parsePurchaseOrder(order, userId))
        .filter((purchase): purchase is Purchase => purchase !== null);
    const spent = spentSteps(purchases);

    let account = findAccount(accounts, username);
    if (!account) {
        account = await createAccount(username, Math.max(0, totalSteps - spent));
    } else if (totalSteps > account.balance + spent) {
        account = await updateAccountBalance(account, totalSteps - spent);
    }

    const equipOrder =
        orders
            .filter((order) => parseEquipOrder(order, userId) !== null)
            .sort((left, right) => right.id - left.id)[0] ?? null;

    return {
        account,
        skins: [...skins].sort((left, right) => left.price - right.price),
        purchases,
        equipOrder,
        totalSteps,
    };
}

export async function buySkin(
    userId: number,
    state: ShopState,
    skin: Skin
): Promise<{ account: Account; purchase: Purchase }> {
    const balance = availableSteps(state.account, state.purchases, state.totalSteps);
    if (balance < skin.price) {
        throw new Error(
            `Not enough steps yet — ${skin.name} costs ${skin.price.toLocaleString()} steps.`
        );
    }

    const previousBalance = state.account.balance;
    const account = await updateAccountBalance(state.account, balance - skin.price);

    try {
        const order = await createOrder(
            purchaseOrderName(userId, skin.id),
            skin.price
        );

        return {
            account,
            purchase: { orderId: order.id, skinId: skin.id, price: order.price },
        };
    } catch (error) {
        // Put the steps back when the purchase record could not be saved.
        await updateAccountBalance(account, previousBalance).catch(() => undefined);
        throw error;
    }
}

export async function equipSkin(
    userId: number,
    state: ShopState,
    skinId: number
): Promise<Order> {
    if (state.equipOrder) {
        return updateOrder(state.equipOrder, equipOrderName(userId, skinId));
    }

    return createOrder(equipOrderName(userId, skinId), 0);
}

function avatarStorageKey(userId: number): string {
    return `${AVATAR_STORAGE_PREFIX}:${userId}`;
}

function notifyAvatarChanged(): void {
    window.dispatchEvent(new Event(SHOP_AVATAR_EVENT));
}

export function cacheAvatar(userId: number, skin: Skin): void {
    const avatar: CachedAvatar = {
        id: skin.id,
        name: skin.name,
        icon: skinIcon(skin.name),
    };
    localStorage.setItem(avatarStorageKey(userId), JSON.stringify(avatar));
    notifyAvatarChanged();
}

export function clearCachedAvatar(userId: number): void {
    localStorage.removeItem(avatarStorageKey(userId));
    notifyAvatarChanged();
}

export function readCachedAvatar(userId: number): CachedAvatar | null {
    if (userId <= 0) {
        return null;
    }

    const raw = localStorage.getItem(avatarStorageKey(userId));
    if (!raw) {
        return null;
    }

    try {
        const avatar = JSON.parse(raw) as CachedAvatar;
        if (
            typeof avatar.id === "number" &&
            typeof avatar.name === "string" &&
            typeof avatar.icon === "string"
        ) {
            return avatar;
        }
    } catch {
        // Ignore a corrupted cache entry.
    }

    return null;
}
