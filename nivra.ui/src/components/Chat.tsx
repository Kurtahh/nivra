import ChatHistory from "./ChatHistory.tsx";
import ChatInput from "./ChatInput.tsx";
import {useState} from "react";


export default function Chat() {
    const [historyVersion, setHistoryVersion] = useState(0);

    return(
        <div>
            <ChatHistory key={historyVersion}></ChatHistory>
            <ChatInput changeHistory={ () => setHistoryVersion((v) => v + 1)}></ChatInput>
        </div>
    );
}