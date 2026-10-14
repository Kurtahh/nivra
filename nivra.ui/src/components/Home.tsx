import { useState } from "react";
import {Session} from "../classes/Session.ts";
import InputSteps from "./InputSteps.tsx";

export default function Home() {
    let currentSession= Session.instance;
    const [todaySteps, setTodaySteps] = useState(currentSession.getTodaySteps());

    return (
        <div className="header">
            <h1> Nivra</h1>
            <p>Hello {currentSession.getUsername()!}, you've taken {todaySteps.toString()} steps today</p>
            <InputSteps onStepsLogged={setTodaySteps} />
        </div>
    );
}