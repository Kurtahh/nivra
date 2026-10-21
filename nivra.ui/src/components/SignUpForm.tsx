import {useState, type ChangeEvent, type FormEvent} from 'react'
import {useNavigate} from 'react-router-dom'
import { loginPath } from "../Paths.ts"

export default function SignUpForm() {
    const [username, setUsername] = useState("")
    const [password, setPassword] = useState("")
    const [message, setMessage] = useState("")

    const navigate = useNavigate();
    const handleSubmit =  async (e: FormEvent<HTMLFormElement>) =>  {
        e.preventDefault()

        let passwordHash = password;
        const response = await fetch('http://localhost:5207/api/User', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify({username, passwordHash})
        });

        if(response.ok) {
            navigate(loginPath);
        }
        else {
            const res = await response.json();
            setMessage(res.message);
        }
    }
    
    function handleUsernameChange(e: ChangeEvent<HTMLInputElement>) {
        setUsername(e.target.value)
    }
    
    function handlePasswordChange(e: ChangeEvent<HTMLInputElement>) {
        setPassword(e.target.value)
    }

    
    return (
        <div className="header">
            <h1 > Sign up</h1>
            <form className="auth-form" onSubmit={handleSubmit}>
                <label htmlFor="username">Username: </label>
                <input type="text" onChange={handleUsernameChange} value={username}></input> <br/><br/>
                <label htmlFor="password">Password: </label>
                <input type="password" onChange={handlePasswordChange} value={password}></input> <br/><br/>
                <input type="submit" value="Submit" />
            </form>
            <br/>
            <div>
                {message == "" ?  "" : message }
            </div>
        </div>
    );
}