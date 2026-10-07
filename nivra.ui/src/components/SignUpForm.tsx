import {useState} from 'react'

export default function SignUpForm() {
    const [username, setUsername] = useState("")
    const [password, setPassword] = useState("")
    const [message, setMessage] = useState("")
    
    const handleSubmit =  async (e) =>  {
        e.preventDefault()
        let passwordHash = password;
        await fetch('http://localhost:5207/api/User', {
            method: 'POST',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify({username, passwordHash})
        }).then(
            res => res.json()
        ).then(
            json =>  {
                setMessage(json.message)
            }
        )
        
        
        
        
        
        
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
            <form >
                <label htmlFor="username">Username: </label>
                <input type="text" onChange={handleUsernameChange} value={username}></input> <br/><br/>
                <label htmlFor="password">Password: </label>
                <input type="password" onChange={handlePasswordChange} value={password}></input> <br/><br/>
                <input type="submit" value="Submit" onClick={handleSubmit}/>
            </form>
            <div>
                {message == "" ?  "" : message }
            </div>
        </div>
        
    );
}