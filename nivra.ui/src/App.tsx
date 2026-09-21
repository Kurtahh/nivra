import './App.css';
import { useEffect, useState } from 'react';
import LoginForm from './LoginForm.tsx';

type StepRecord = {
  date: string;
  stepCount: number;
  userId: number;
};

export default function App() {
  const [records, setRecords] = useState<StepRecord[]>([]);

  useEffect(() => {
    fetch('http://localhost:5207/api/DailySteps')
    .then(res => res.json())
    .then(data => setRecords(data));
  }, []);

  return (
      <div className="header">
        <h1> Nivra</h1>
        <p>Sveiki!</p>
        <ul>
          {records.map((r, i) => (
            <li key={i}>{r.date} — {r.stepCount} steps (user {r.userId})</li>
          ))}
        </ul>
        <LoginForm /> 
          
      </div>
  );
}
