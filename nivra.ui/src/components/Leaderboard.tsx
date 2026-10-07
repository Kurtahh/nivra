import React, { useEffect, useState } from 'react';

interface LeaderboardEntry {
    userId: number;
    username: string;
    totalSteps: number;
    rank: number;
}

export const Leaderboard: React.FC = () => {
    const [entries, setEntries] = useState<LeaderboardEntry[]>([]);
    const [period, setPeriod] = useState<string>('Weekly');
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        setLoading(true);
        fetch(`http://localhost:5207/api/leaderboard?period=${period}`)
            .then((res) => {
                if (!res.ok) throw new Error('Failed to fetch leaderboard');
                return res.json();
            })
            .then((data) => {
                setEntries(data);
                setLoading(false);
            })
            .catch((err) => {
                console.error('Error fetching leaderboard:', err);
                setLoading(false);
            });
    }, [period]);

    return (
        <div style={{ padding: '20px', maxWidth: '600px', margin: '0 auto' }}>
            <h2>Leaderboard</h2>

            <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '15px' }}>
                <select value={period} onChange={(e) => setPeriod(e.target.value)}>
                    <option value="Daily">Daily</option>
                    <option value="Weekly">Weekly</option>
                    <option value="Monthly">Monthly</option>
                    <option value="AllTime">All Time</option>
                </select>
                
            </div>

            {loading ? (
                <p>Loading leaderboard...</p>
            ) : entries.length === 0 ? (
                <p>No steps logged yet!</p>
            ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                    <thead>
                    <tr style={{ borderBottom: '2px solid #ccc' }}>
                        <th style={{ padding: '8px' }}>Rank</th>
                        <th style={{ padding: '8px' }}>User</th>
                        <th style={{ padding: '8px' }}>Total Steps</th>
                    </tr>
                    </thead>
                    <tbody>
                    {entries.map((entry) => (
                        <tr key={entry.userId} style={{ borderBottom: '1px solid #eee' }}>
                            <td style={{ padding: '8px', fontWeight: 'bold' }}>#{entry.rank}</td>
                            <td style={{ padding: '8px' }}>{entry.username}</td>
                            <td style={{ padding: '8px' }}>{entry.totalSteps.toLocaleString()}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};