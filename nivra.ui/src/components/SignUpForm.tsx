
import {useState} from 'react'
export default function SignUpForm() {


    const [username, setUsername] = useState("")
    const [password, setPassword] = useState("")
    
    function handleSubmit(e) {
        e.preventDefault()
        let id = 3
        let passwordHash = password;
        const response = fetch('http://localhost:5207/api/User', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify({username, passwordHash})
        })
        
        if (!response) {
            throw new Error("Couldn't signup");
        }
        
        console.log(username + " " + password );
    }
    
    function handleUsernameChange(e) {

        setUsername(e.target.value)
    }
    
    function handlePasswordChange(e) {
        setPassword(e.target.value)
        
    }

    
    return (
        <div className="header">
            <h1 > Sign up</h1>
            <form onClick={handleSubmit}>
                <label htmlFor="username">Username: </label>
                <input type="text" onChange={handleUsernameChange} value={username}></input> <br/><br/>
                <label htmlFor="password">Password: </label>
                <input type="password" onChange={handlePasswordChange} value={password}></input> <br/><br/>
                <input type="submit" value="Submit"/>
            </form>
        </div>
        
    );
}