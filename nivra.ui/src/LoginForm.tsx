export default function LoginForm() {
    return (
        <form>
            <label htmlFor="username">Username: </label>
            <input type="text"></input> <br/><br/>
            <label htmlFor="password">Password: </label>
            <input type="text"></input> <br/><br/>
            <input type="submit" value="Submit"/>
        </form>
    );
}