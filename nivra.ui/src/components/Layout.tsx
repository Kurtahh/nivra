import { useEffect, useState } from "react";
import {Link, Outlet} from 'react-router-dom';
import {Session} from "../classes/Session.ts";
import {readCachedAvatar, SHOP_AVATAR_EVENT} from "../classes/ShopApi.ts";

function NavAvatar() {
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
        <li className="nav-avatar" title={`${avatar.name} avatar`}>
            <span role="img" aria-label={`Profile picture: ${avatar.name}`}>
                {avatar.icon}
            </span>
        </li>
    );
}

export default function Layout({routes}: {routes: [string, string][]}) {
    
    return (
        <div className="app-shell">
            <nav className="site-nav">
                <ul className="nav-bar">
                    <NavAvatar />
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
        </div>
    )
}