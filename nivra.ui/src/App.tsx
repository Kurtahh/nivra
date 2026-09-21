import './App.css';
import LoginForm from './LoginForm.tsx';
import Home from './Home.tsx';

import {BrowserRouter, Routes, Route, Link} from 'react-router-dom';

export default function App() {

  return (
      <BrowserRouter>
          <nav>
              <Link to="/">Home</Link>
              <Link to="/login">Login</Link>
          </nav>
          <Routes>
              <Route path="/" element={<Home />}></Route>
              <Route path="/login" element={<LoginForm />}> </Route>
          </Routes>
          
      </BrowserRouter>
  );
}
