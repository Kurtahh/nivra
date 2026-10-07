import './App.css';
import LoginForm from './components/LoginForm.tsx';
import Home from './components/Home.tsx';
import SignUpForm from "./components/SignUpForm.tsx";
import InputSteps from "./components/InputSteps.tsx";
import { Leaderboard } from './components/Leaderboard';

import {BrowserRouter, Routes, Route, Link} from 'react-router-dom';

export default function App() {
    
  return (
      <BrowserRouter>
          <nav style={{ display: 'flex', gap: '20px', marginBottom: '20px' }}>
              <Link to="/">Home</Link>
              <Link to="/login">Login</Link>
              <Link to="/signup">Sign Up</Link>
              <Link to="/leaderboard">Leaderboard</Link>
          </nav>
          <Routes>
              <Route path="/" element={<><Home /><InputSteps /></>}></Route>
              <Route path="/leaderboard" element={<Leaderboard />} />
              <Route path="/login" element={<LoginForm />}> </Route>
              <Route path="/signup" element={<SignUpForm />}> </Route>
          </Routes>
          
      </BrowserRouter>
  );
}
