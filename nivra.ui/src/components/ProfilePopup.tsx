import { useEffect, useState } from "react";
import { skinIcon } from "../classes/ShopApi.ts";

type ProfileInfo = {
    createdAt: string;
    accountAge: number;
    stepsToday: number;
    stepsThisMonth: number;
    equippedSkin: string | null;
};

export default function ProfilePopup({ username, onClose }: { username: string; onClose: () => void }) {
    const [info, setInfo] = useState<ProfileInfo | null>(null);
    const [failed, setFailed] = useState(false);

    useEffect(() => {
        async function loadProfile() {
            try {
                const res = await fetch(`http://localhost:5207/api/User/Profile/${encodeURIComponent(username)}`);
                if (!res.ok) throw new Error("Failed to fetch profile");
                // <-- the controller's response (ProfileInfoResponse) arrives here as `info`
                setInfo(await res.json());
            } catch (err) {
                console.error("Error fetching profile: ", err);
                setFailed(true);
            }
        }
        loadProfile();
    }, [username]);

    useEffect(() => {
        const onKeyDown = (e: KeyboardEvent) => {
            if (e.key === "Escape") onClose();
        };
        window.addEventListener("keydown", onKeyDown);
        return () => window.removeEventListener("keydown", onKeyDown);
    }, [onClose]);

    const icon = info?.equippedSkin ? skinIcon(info.equippedSkin) : "🏃";

    return (
        <div className="profile-backdrop" onClick={onClose}>
            <div
                className="profile-popup"
                role="dialog"
                aria-modal="true"
                aria-label={`${username}'s profile`}
                onClick={(e) => e.stopPropagation()}
            >
                <span className="avatar avatar-large" role="img" aria-label="Profile picture">
                    {icon}
                </span>
                <h2>{username}</h2>

                {failed ? (
                    <p role="alert">Could not load this profile.</p>
                ) : info ? (
                    <dl className="profile-stats">
                        <dt>Account age</dt>
                        <dd>{info.accountAge.toLocaleString()} days</dd>
                        <dt>Steps today</dt>
                        <dd>{info.stepsToday.toLocaleString()}</dd>
                        <dt>Steps past month</dt>
                        <dd>{info.stepsThisMonth.toLocaleString()}</dd>
                    </dl>
                ) : (
                    <p className="muted">Loading...</p>
                )}

                <button onClick={onClose}>Close</button>
            </div>
        </div>
    );
}