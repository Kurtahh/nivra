import {useState, type ChangeEvent, type FormEvent} from "react";
import { useNavigate, Link } from "react-router-dom";
import {Session} from "../classes/Session.ts"
import { homePath, signupPath } from "../Paths.ts"

export default function LoginForm() {
    let navigate = useNavigate();
    const  handleSubmit = async (e: FormEvent<HTMLFormElement>) => {

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
                let currentSession = Session.instance;
                currentSession.login(username, items[2]);
                navigate(homePath);
            }
            
        }
    }
    
    const [username, setUsername] = useState("")
    const [password, setPassword] = useState("")
    
    function handleUsernameChange(e: ChangeEvent<HTMLInputElement>) {
        setUsername(e.target.value)
    }

    function handlePasswordChange(e: ChangeEvent<HTMLInputElement>) {
        setPassword(e.target.value)
    }

    return (
        <div className="header">
            <h1 >Log in</h1>
            <form className="auth-form" onSubmit={handleSubmit}>
                <label htmlFor="username">Username: </label>
                <input type="text" onChange={handleUsernameChange} value={username}></input> <br/><br/>
                <label htmlFor="password">Password: </label>
                <input type="password" onChange={handlePasswordChange} value={password}></input> <br/><br/>
                <input type="submit" value="Submit" />
            </form>
            <br/>
            <Link to={signupPath}>Don't have an account yet?</Link>
        </div>
    );

}