import {useState} from "react";

export default function LoginForm() {

    const  handleSubmit = async (e) => {

        e.preventDefault()
        /*
                    const response = await fetch('api/login'), {
                        method: 'POST',
                        headers: {
                            'Content-type': 'application/json',
                        },
                        body: JSON.stringify({username, password})
                    }
        */
        
        try {
            const response = await fetch('api/DailySteps', {
                method: 'GET',
                headers: {
                    'Content-type': 'application/json',
                },
            })

            if (!response.ok) {
                throw new Error('Login failed');
            }

            const data = await response.json()
            console.log(data)
        } catch {
            throw new Error('Login failed');           
        }
    }
    
    const [username, setUsername] = useState("")
    const [password, setPassword] = useState("")
    
    function handleUsernameChange(e) {

        setUsername(e.target.value)
    }

    function handlePasswordChange(e) {
        setPassword(e.target.value)

    }

    return (
        <div className="header">
            <h1 >Log in</h1>
            <form >
                <label htmlFor="username">Username: </label>
                <input type="text" onChange={handleUsernameChange} value={username}></input> <br/><br/>
                <label htmlFor="password">Password: </label>
                <input type="password" onChange={handlePasswordChange} value={password}></input> <br/><br/>
                <input type="submit" value="Submit" onClick={handleSubmit}/>
            </form>
        </div>
    );

}