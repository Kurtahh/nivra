import {useState} from "react";
import {Session} from "../classes/Session.ts"

export default function LoginForm() {

    const  handleSubmit = async (e) => {

        e.preventDefault()
        let passwordHash = password;
        
        let items;
        let dataIsLoaded;
        await fetch("http://localhost:5207/api/User/Authenticate", {
            method: "POST",
            headers: {
                'Content-type' : "application/json"
            },
            body: JSON.stringify({username, passwordHash})
        }).then((res) => res.json())
          .then((json) => {
              items = json;
              dataIsLoaded = true;
          })
        
        
        
        
        if (dataIsLoaded && items != null) {
            console.log(items[0]);
            if (items[1] === "true") {
                let current_session = new Session();
                current_session.login(username);
            }
            
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