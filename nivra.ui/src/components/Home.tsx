import {Session} from "../classes/Session.ts";


export default function Home() {
    let currentSession= Session.instance;
    
    return (
        <div className="header">
            <h1> Nivra</h1>
            <p>Sveiki {currentSession.getUsername()!} Today steps: {currentSession.getTodaySteps().toString()}</p>
        </div>
    );
}