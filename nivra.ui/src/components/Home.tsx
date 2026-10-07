import { useState } from "react";
import {Session} from "../classes/Session.ts";
import InputSteps from "./InputSteps.tsx";

export default function Home() {
    let currentSession= Session.instance;
    const [todaySteps, setTodaySteps] = useState(currentSession.getTodaySteps());

    return (
        <div className="header">
            <h1> Nivra</h1>
            <p>Sveiki {currentSession.getUsername()!} Today steps: {todaySteps.toString()}</p>
            <InputSteps onStepsLogged={setTodaySteps} />
        </div>
    );
}