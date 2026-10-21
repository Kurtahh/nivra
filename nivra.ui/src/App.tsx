import './App.css';
import LoginForm from './components/LoginForm.tsx';
import Home from './components/Home.tsx';
import SignUpForm from "./components/SignUpForm.tsx";
import { homePath, leaderboardPath, signupPath, loginPath, chatPath } from "./Paths.ts"
import Layout from "./components/Layout.tsx";
import Chat from "./components/Chat.tsx";

import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Leaderboard } from "./components/Leaderboard.tsx";

const postLoginRoutes: [string, string][] = [
    [homePath, "Home"],
    [leaderboardPath, "Leaderboard"],
    [chatPath, "Chat"]
];

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path={loginPath} element={<LoginForm />} />
                <Route element={<Layout routes={[[loginPath, "Login"]]} />} >
                    <Route path={signupPath} element={<SignUpForm />} />
                </Route>
                <Route element={<Layout routes={postLoginRoutes} />} >
                    <Route path={homePath} element={<Home />} />
                    <Route path={leaderboardPath} element={<Leaderboard />} />
                    <Route path={chatPath} element={<Chat />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}
