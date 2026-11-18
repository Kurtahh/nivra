import { useEffect, useState } from "react";
import {Link, Outlet} from 'react-router-dom';
import {Session} from "../classes/Session.ts";
import {readCachedAvatar, SHOP_AVATAR_EVENT} from "../classes/ShopApi.ts";
import ProfilePopup from "./ProfilePopup.tsx";

function NavAvatar({ onOpen }: { onOpen: () => void }) {
    const userId = Number(Session.instance.getId());
    const [avatar, setAvatar] = useState(() => readCachedAvatar(userId));

    useEffect(() => {
        const updateAvatar = () => setAvatar(readCachedAvatar(userId));

        window.addEventListener(SHOP_AVATAR_EVENT, updateAvatar);
        window.addEventListener("storage", updateAvatar);

        return () => {
            window.removeEventListener(SHOP_AVATAR_EVENT, updateAvatar);
            window.removeEventListener("storage", updateAvatar);
        };
    }, [userId]);

    if (!avatar) {
        return null;
    }

    return (
        <li>
            <button
                type="button"
                className="nav-avatar"
                onClick={onOpen}
                title={`${avatar.name} avatar`}
                aria-label="Open your profile"
            >
                <span aria-hidden="true">{avatar.icon}</span>
            </button>
        </li>
    );
}

export default function Layout({routes}: {routes: [string, string][]}) {
    const [profileOpen, setProfileOpen] = useState(false);

    return (
        <div className="app-shell">
            <nav className="site-nav">
                <ul className="nav-bar">
                    <NavAvatar onOpen={() => setProfileOpen(true)} />
                    {routes.map(([route, title]) =>
                        <li key={route}>
                            <Link className="nav-link" to={route}>{title}</Link>
                        </li>
                    )}
                </ul>
            </nav>
            <main className="app-main">
                <Outlet/>
            </main>
            {profileOpen && (
                <ProfilePopup
                    username={String(Session.instance.getUsername())}
                    onClose={() => setProfileOpen(false)}
                />
            )}
        </div>
    )
}