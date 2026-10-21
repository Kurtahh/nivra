import { useCallback, useEffect, useState } from "react";

type HealthTip = {
    id: number;
    content: string;
    category: number;
    author: string | null;
    sourceUrl: string | null;
};

const REFRESH_INTERVAL_MS = 30000;

export default function HealthTipWidget() {
    const [tip, setTip] = useState<HealthTip | null>(null);
    const [failed, setFailed] = useState(false);

    const loadTip = useCallback(async () => {
        try {
            const res = await fetch("http://localhost:5207/api/HealthTip");
            if(!res.ok) throw new Error("Failed to fetch health tip");
            const data: HealthTip = await res.json();
            setTip(data);
        } catch (err) {
            console.error("Error fetching health tip: ", err);
            setFailed(true);
        }
    }, []);

    useEffect(() => {
        loadTip();
        const timer = setInterval(loadTip, REFRESH_INTERVAL_MS);
        return () => clearInterval(timer);
    }, [loadTip]);

    let text: string;
    if (tip != null) {
        text = tip.content;
    } else if (failed) {
        text = "Health tips are unavailable at the moment.";
    } else {
        text = "Loading a health tip...";
    }

    return (
        <div className="health-tip" onClick={loadTip} title="Click for another tip">
            <p>{text}</p>
        </div>
    );
}