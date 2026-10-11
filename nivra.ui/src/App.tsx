import './App.css';
import LoginForm from './components/LoginForm.tsx';
import Home from './components/Home.tsx';
import SignUpForm from "./components/SignUpForm.tsx";

import {BrowserRouter, Routes, Route, Link} from 'react-router-dom';
import {Leaderboard} from "./components/Leaderboard.tsx";

export default function App() {
    
  return (
      <BrowserRouter>
          <nav style={{ display: 'flex', gap: '20px', marginBottom: '20px' }}>
              <Link to="/">Login</Link>
              <Link to="/signup">Sign Up</Link>
              <Link to="/home">Home</Link>
              <Link to="/leaderboard">Leaderboard</Link>
          </nav>
          <Routes>
              <Route path="/" element={<><LoginForm /></>}></Route>
              <Route path="/signup" element={<SignUpForm />}> </Route>
              <Route path="/home" element={<Home />}> </Route>
              <Route path="/leaderboard" element={<Leaderboard /> }> </Route>
          </Routes>
          
      </BrowserRouter>
  );
}
