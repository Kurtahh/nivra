import { useState } from "react";
import { Session } from "../classes/Session";

type Props = { onStepsLogged: (value: number) => void };

export default function InputSteps({onStepsLogged}: Props) {
    const [steps, setSteps] = useState("")
    const [errorMsg, setErrorMsg] = useState("")

    async function handleSubmit(e) {
        e.preventDefault()
        const response = await fetch('http://localhost:5207/api/Steps', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ 
                stepCount: Number(steps),
                userId: Number(Session.instance.getId())
            }) 
        });

        if (response.ok) {
            setSteps("")
            setErrorMsg("")

            const newSteps = await Session.instance.fetchTodaySteps();
            onStepsLogged(newSteps);
        }
        else {
            const msg = await response.text();
            setErrorMsg(msg)
        }
    }

    return (
        <div className="centered-form">
            <label>
                Įveskite nueitų žingsnių kiekį
            </label>
            <br/>
            <form onSubmit={handleSubmit}>
                <input 
                    type="number"
                    value={steps}
                    onChange={(e) => setSteps(e.target.value)}
                />
                <button>
                    Įrašyti
                </button>
            </form>
            <p>
                {errorMsg}
            </p>
        </div>
    );
}