import ChatHistory from "./ChatHistory.tsx";
import ChatInput from "./ChatInput.tsx";
import {useState} from "react";


export default function Chat() {
    const [historyVersion, setHistoryVersion] = useState(0);

    return(
        <section className="chat-page">
            <h1 className="page-title">Chat</h1>
            <ChatHistory key={historyVersion}></ChatHistory>
            <ChatInput changeHistory={ () => setHistoryVersion((v) => v + 1)}></ChatInput>
        </section>
    );
}