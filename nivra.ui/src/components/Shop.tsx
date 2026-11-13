import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Session } from "../classes/Session";
import {
    type ShopState,
    type Skin,
    availableSteps,
    buySkin,
    cacheAvatar,
    clearCachedAvatar,
    deleteOrder,
    equipSkin,
    loadShop,
    maxSteps,
    ownedSkinIds,
    parseEquipOrder,
    skinIcon,
    spentSteps,
} from "../classes/ShopApi";
import { homePath } from "../Paths.ts";

function errorMessage(error: unknown): string {
    return error instanceof Error ? error.message : "Something went wrong. Please try again.";
}

export default function Shop() {
    const session = Session.instance;
    const userId = Number(session.getId());
    const username = String(session.getUsername());
    const isLoggedIn = userId > 0 && username !== "";

    const [state, setState] = useState<ShopState | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [busySkinId, setBusySkinId] = useState<number | null>(null);
    const [reloadToken, setReloadToken] = useState(0);

    useEffect(() => {
        if (!isLoggedIn) {
            return;
        }

        let active = true;

        (async () => {
            try {
                const shop = await loadShop(userId, username);
                if (!active) {
                    return;
                }

                setState(shop);
                setError("");

                const equippedSkinId = shop.equipOrder
                    ? parseEquipOrder(shop.equipOrder, userId)
                    : null;
                const equippedSkin = shop.skins.find((skin) => skin.id === equippedSkinId);
                if (equippedSkin) {
                    cacheAvatar(userId, equippedSkin);
                } else {
                    clearCachedAvatar(userId);
                }
            } catch (loadError) {
                if (active) {
                    setError(errorMessage(loadError));
                }
            } finally {
                if (active) {
                    setLoading(false);
                }
            }
        })();

        return () => {
            active = false;
        };
    }, [isLoggedIn, userId, username, reloadToken]);

    function retry() {
        setLoading(true);
        setError("");
        setReloadToken((token) => token + 1);
    }

    async function handleBuy(skin: Skin) {
        if (!state || busySkinId !== null) {
            return;
        }

        setBusySkinId(skin.id);
        setError("");

        try {
            const { account, purchase } = await buySkin(userId, state, skin);
            setState((current) =>
                current
                    ? {
                          ...current,
                          account,
                          purchases: [...current.purchases, purchase],
                      }
                    : current
            );
        } catch (buyError) {
            setError(errorMessage(buyError));
        } finally {
            setBusySkinId(null);
        }
    }

    async function handleEquip(skin: Skin) {
        if (!state || busySkinId !== null) {
            return;
        }

        setBusySkinId(skin.id);
        setError("");

        try {
            const equipOrder = await equipSkin(userId, state, skin.id);
            setState((current) =>
                current ? { ...current, equipOrder } : current
            );
            cacheAvatar(userId, skin);
        } catch (equipError) {
            setError(errorMessage(equipError));
        } finally {
            setBusySkinId(null);
        }
    }

    async function handleUnequip() {
        if (!state?.equipOrder || busySkinId !== null) {
            return;
        }

        setBusySkinId(0);
        setError("");

        try {
            await deleteOrder(state.equipOrder);
            setState((current) =>
                current ? { ...current, equipOrder: null } : current
            );
            clearCachedAvatar(userId);
        } catch (unequipError) {
            setError(errorMessage(unequipError));
        } finally {
            setBusySkinId(null);
        }
    }

    if (!isLoggedIn) {
        return (
            <section className="shop">
                <div className="shop-empty">
                    <h2>Avatar shop</h2>
                    <p className="muted">Log in to spend your steps on a new avatar.</p>
                    <Link to={homePath}>Back to home</Link>
                </div>
            </section>
        );
    }

    if (loading) {
        return (
            <section className="shop">
                <p className="muted">Loading the avatar shop…</p>
            </section>
        );
    }

    if (!state) {
        return (
            <section className="shop">
                <div className="shop-empty" role="alert">
                    <h2>Avatar shop</h2>
                    <p>{error || "Could not load the shop."}</p>
                    <button type="button" onClick={retry}>
                        Try again
                    </button>
                </div>
            </section>
        );
    }

    const spent = spentSteps(state.purchases);
    const balance = availableSteps(state.account, state.purchases, state.totalSteps);
    const max = maxSteps(state.account, state.purchases, state.totalSteps);
    const owned = ownedSkinIds(state);
    const equippedSkinId = state.equipOrder
        ? parseEquipOrder(state.equipOrder, userId)
        : null;
    const equippedSkin = state.skins.find((skin) => skin.id === equippedSkinId);

    return (
        <section className="shop">
            <header className="shop-header">
                <div>
                    <h2>Avatar shop</h2>
                    <p className="muted">
                        Spend the steps you have walked on a profile picture.
                    </p>
                </div>
                <div className="shop-wallet">
                    <span className="shop-balance">{balance.toLocaleString()} steps</span>
                    <span className="muted">
                        {spent.toLocaleString()} spent · {max.toLocaleString()} earned
                    </span>
                </div>
            </header>

            <div className="shop-profile">
                <span className="avatar avatar-large" role="img" aria-label={`Profile picture: ${equippedSkin?.name ?? "Default walker"}`}>
                    {equippedSkin ? skinIcon(equippedSkin.name) : "🏃"}
                </span>
                <div className="shop-profile-copy">
                    <span className="shop-eyebrow">Your profile picture</span>
                    <strong>{equippedSkin?.name ?? "Default walker"}</strong>
                    {equippedSkin && (
                        <button
                            type="button"
                            className="shop-link-button"
                            onClick={handleUnequip}
                            disabled={busySkinId !== null}
                        >
                            Reset to default
                        </button>
                    )}
                </div>
            </div>

            {error && <p role="alert">{error}</p>}

            {state.skins.length === 0 ? (
                <div className="shop-empty">
                    <p className="muted">The shop has no avatars yet.</p>
                </div>
            ) : (
                <ul className="shop-grid">
                    {state.skins.map((skin) => {
                        const isOwned = owned.has(skin.id);
                        const isEquipped = skin.id === equippedSkinId;
                        const canAfford = balance >= skin.price;
                        const busy = busySkinId === skin.id;

                        return (
                            <li
                                key={skin.id}
                                className={`shop-card${isOwned ? " owned" : ""}${
                                    isEquipped ? " equipped" : ""
                                }`}
                            >
                                <span className="shop-icon" aria-hidden="true">
                                    {skinIcon(skin.name)}
                                </span>
                                <h3>{skin.name}</h3>
                                <p className="shop-price">
                                    {skin.price === 0
                                        ? "Free"
                                        : `${skin.price.toLocaleString()} steps`}
                                </p>
                                {isOwned && (
                                    <span className="shop-status">
                                        {isEquipped ? "Equipped" : "Owned"}
                                    </span>
                                )}
                                {isOwned ? (
                                    <button
                                        type="button"
                                        onClick={() => handleEquip(skin)}
                                        disabled={isEquipped || busySkinId !== null}
                                    >
                                        {isEquipped
                                            ? "Selected"
                                            : busy
                                              ? "Selecting…"
                                              : "Use avatar"}
                                    </button>
                                ) : (
                                    <button
                                        type="button"
                                        onClick={() => handleBuy(skin)}
                                        disabled={!canAfford || busySkinId !== null}
                                    >
                                        {busy
                                            ? "Buying…"
                                            : canAfford
                                              ? "Buy"
                                              : "Not enough steps"}
                                    </button>
                                )}
                            </li>
                        );
                    })}
                </ul>
            )}
        </section>
    );
}
