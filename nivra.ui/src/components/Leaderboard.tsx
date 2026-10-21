import { useEffect, useState } from 'react';

interface LeaderboardEntry {
    userId: number;
    username: string;
    totalSteps: number;
    rank: number;
}

export const Leaderboard = () => {
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
        <section className="leaderboard">
            <h2>Leaderboard</h2>

            <div className="leaderboard-controls">
                <label htmlFor="leaderboard-period">Period</label>
                <select
                    id="leaderboard-period"
                    value={period}
                    onChange={(e) => setPeriod(e.target.value)}
                >
                    <option value="Daily">Daily</option>
                    <option value="Weekly">Weekly</option>
                    <option value="Monthly">Monthly</option>
                    <option value="AllTime">All Time</option>
                </select>
            </div>

            {loading ? (
                <p className="muted">Loading leaderboard...</p>
            ) : entries.length === 0 ? (
                <p className="muted">No steps logged yet!</p>
            ) : (
                <table className="leaderboard-table">
                    <thead>
                    <tr>
                        <th>Rank</th>
                        <th>User</th>
                        <th>Total Steps</th>
                    </tr>
                    </thead>
                    <tbody>
                    {entries.map((entry) => (
                        <tr key={entry.userId}>
                            <td className="rank-cell">#{entry.rank}</td>
                            <td>{entry.username}</td>
                            <td>{entry.totalSteps.toLocaleString()}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            )}
        </section>
    );
};
