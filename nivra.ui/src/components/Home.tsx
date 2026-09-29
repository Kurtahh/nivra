import {useEffect, useState} from "react";
import {Session} from "../classes/Session.ts";

type StepEntry = {
    date: string;
    stepCount: number;
    userId: number;
};

export default function Home() {
    const [records, setRecords] = useState<StepEntry[]>([]);
    let currentSession= Session.instance;
    
    useEffect(() => {
        fetch('http://localhost:5207/api/DailySteps')
            .then(res => res.json())
            .then(data => setRecords(data));
    }, []);
    return (
        <div className="header">
            <h1> Nivra</h1>
            <p>Sveiki {currentSession.getUsername()!}</p>
            <ul>
                {records.map((r, i) => (
                    <li key={i}>{r.date} — {r.stepCount} steps (user {r.userId})</li>
                ))}
            </ul>
        </div>
    );
}