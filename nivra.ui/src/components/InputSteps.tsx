import { useState } from "react";

export default function InputSteps() {
    const [steps, setSteps] = useState("")
    const [errorMsg, setErrorMsg] = useState("")

    async function handleSubmit(e) {
        e.preventDefault()
        const response = await fetch('http://localhost:5207/api/StepEntry', {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(Number(steps))
        });

        if (response.ok) {
            setSteps("")
            setErrorMsg("")
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