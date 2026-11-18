import { useEffect, useState } from "react";
import { Session } from "../classes/Session.ts";
import { readCachedAvatar } from "../classes/ShopApi.ts";

type ProfileInfo = {
    createdAt: string;
    accountAge: number;
    stepsThisMonth: number;
};

export default function ProfilePopup({ onClose, todaySteps }: { onClose: () => void; todaySteps: number }) {
    const session = Session.instance;
    const userId = Number(session.getId());
    const username = session.getUsername().toString();
    const avatar = readCachedAvatar(userId);
    const [info, setInfo] = useState<ProfileInfo | null>(null);
    const [failed, setFailed] = useState(false);

    useEffect(() => {
        async function loadProfile() {
            try {
                // TODO: point this at your endpoint
                const res = await fetch(`http://localhost:5207/api/User/Profile/${encodeURIComponent(username)}`);
                if (!res.ok) throw new Error("Failed to fetch profile");
                setInfo(await res.json());
            } catch (err) {
                console.error("Error fetching profile: ", err);
                setFailed(true);
            }
        }
        loadProfile();
    }, [userId]);

    useEffect(() => {
        const onKeyDown = (e: KeyboardEvent) => {
            if (e.key === "Escape") onClose();
        };
        window.addEventListener("keydown", onKeyDown);
        return () => window.removeEventListener("keydown", onKeyDown);
    }, [onClose]);

    return (
        <div className="profile-backdrop" onClick={onClose}>
            <div
                className="profile-popup"
                role="dialog"
                aria-modal="true"
                aria-label="Your profile"
                onClick={(e) => e.stopPropagation()}
            >
                <span className="avatar avatar-large" role="img" aria-label="Your profile picture">
                    {avatar?.icon ?? "🏃"}
                </span>
                <h2>{session.getUsername().toString()}</h2>

                {failed ? (
                    <p role="alert">Could not load your profile.</p>
                ) : info ? (
                    <dl className="profile-stats">
                        <dt>Account age</dt>
                        <dd>{info.accountAge.toLocaleString()} days</dd>
                        <dt>Steps today</dt>
                        <dd>{todaySteps.toLocaleString()}</dd>
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