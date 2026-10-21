import { useEffect, useState } from "react";

type HealthTip = {
    id: number;
    content: string;
    category: number;
    author: string | null;
    sourceUrl: string | null;
};

export default function HealthTipWidget() {
    const [tip, setTip] = useState<HealthTip | null>(null);

    useEffect(() => {
        fetch("http://localhost:5207/api/HealthTip")
            .then((res) => {
                if (!res.ok) throw new Error("Failed to fetch health tip");
                return res.json();
            })
            .then((data: HealthTip) => setTip(data))
            .catch((err) => console.error("Error fetching health tip: ", err));
    }, []);

    return (
        <div className="health-tip">
            <p>{tip == null ? "Loading a health tip..." : tip.content}</p>
        </div>
    );
}