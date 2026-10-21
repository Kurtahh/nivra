import { useState } from "react";
import {Session} from "../classes/Session.ts";

type Props = { changeHistory?: () => void };

export default function ChatInput({changeHistory}: Props) {
    
    const [message, setMessage] = useState("");
    const [errorMsg, setErrorMsg] = useState("");

    function isBlank(s: string): boolean {
        return /^\s*$/.test(s);
    }

    async function handleSubmit(e: { preventDefault: () => void; }) {
        e.preventDefault();
        setErrorMsg("");

        const current_session = Session.instance;

        try {
            const response = await fetch('http://localhost:5207/api/Chat/SendMessage', {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    message: message,
                    userId: current_session.getId()
                })
            });

            if (response.ok) {
                setMessage("")
                setErrorMsg("")
                changeHistory?.();
            } else {
                setErrorMsg(await response.text())
            }
        }  catch {
            setErrorMsg("Could not connect to the server. Message was not sent.")
        }

    }
    return (
        <div className="centered-form chat-composer">
            <label htmlFor="chat-message">Send a message to the community:</label>
            <form className="chat-form" onSubmit={handleSubmit}>
                <input
                    id="chat-message"
                    type="text"
                    placeholder="Type a message..."
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                />
                <button type="submit" disabled={isBlank(message)}>Send</button>
            </form>
            <p className="form-error" role="alert">{errorMsg}</p>
        </div>
    );
}
