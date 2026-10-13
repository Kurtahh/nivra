import './App.css';
import LoginForm from './components/LoginForm.tsx';
import Home from './components/Home.tsx';
import SignUpForm from "./components/SignUpForm.tsx";
import Layout from "./components/Layout.tsx";

import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Leaderboard } from "./components/Leaderboard.tsx";

const loginPath = "/"
const signupPath = "/signup"
const homePath = "/home"
const leaderboardPath = "/leaderboard"

const postLoginRoutes: [string, string][] = [
    [homePath, "Home"],
    [leaderboardPath, "Leaderboard"]
];

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path={loginPath} element={<LoginForm />} />
                <Route path={signupPath} element={<SignUpForm />} />
                <Route element={<Layout routes={postLoginRoutes} />} >
                    <Route path={homePath} element={<Home />} />
                    <Route path={leaderboardPath} element={<Leaderboard />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}
