import { useState } from "react";
import { Link } from "react-router-dom";
import {Session} from "../classes/Session.ts";
import {readCachedAvatar} from "../classes/ShopApi.ts";
import {shopPath} from "../Paths.ts";
import InputSteps from "./InputSteps.tsx";
import HealthTipWidget from "./HealthTipWidget.tsx";
import ProfilePopup from "./ProfilePopup.tsx"

export default function Home() {
    const [profileOpen, setProfileOpen] = useState(false);
    const currentSession= Session.instance;
    const [todaySteps, setTodaySteps] = useState(currentSession.getTodaySteps());
    const userId = Number(currentSession.getId());
    const avatar = readCachedAvatar(userId);


    return (
        <div className="home">
            <button
                className="avatar profile-trigger"
                onClick={() => setProfileOpen(true)}
                aria-label="Open your profile"
            >
                {avatar?.icon ?? "🏃"}
            </button>

            <div className="header">
                <span className="avatar avatar-large home-avatar" role="img" aria-label="Your profile picture">
                    {avatar?.icon ?? "🏃"}
                </span>
                <h1> Nivra</h1>
                <p>Hello {currentSession.getUsername()!}, you've taken {todaySteps.toString()} steps today</p>
                <InputSteps onStepsLogged={setTodaySteps} />
                <Link className="home-shop-link" to={shopPath}>Change avatar in the shop</Link>
                <HealthTipWidget />
            </div>
            {profileOpen && <ProfilePopup username={currentSession.getUsername().toString()} onClose={() => setProfileOpen(false)} />}
        </div>
    );
}