import './App.css';
import LoginForm from './components/LoginForm.tsx';
import Home from './components/Home.tsx';
import SignUpForm from "./components/SignUpForm.tsx";

import {BrowserRouter, Routes, Route, Link} from 'react-router-dom';

export default function App() {
    
  return (
      <BrowserRouter>
          <nav>
              <Link to="/">Home</Link>
              <Link to="/login">Login</Link>
              <Link to="/signup">Sign Up</Link>
          </nav>
          <Routes>
              <Route path="/" element={<Home />}></Route>
              <Route path="/login" element={<LoginForm />}> </Route>
              <Route path="/signup" element={<SignUpForm />}> </Route>
          </Routes>
          
      </BrowserRouter>
  );
}
