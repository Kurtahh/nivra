import { useEffect, useState, type FormEvent } from "react";
import { Session } from "../classes/Session.ts";
import "./Blog.css";

interface BlogPost {
    id: number;
    title: string;
    content: string;
    category: string;
    createdAt: string;
    userId: number;
    username: string;
}

export default function Blog() {
    const [posts, setPosts] = useState<BlogPost[]>([]);
    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [category, setCategory] = useState("Recipes");
    const [error, setError] = useState("");

    const session = Session.instance;

    async function loadPosts() {
        const response = await fetch("http://localhost:5207/api/Blog");
        const data = await response.json();
        setPosts(data);
    }

    useEffect(() => {
        loadPosts().catch(error =>
            console.error("Error loading blog posts:", error)
        );
    }, []);

    async function handleSubmit(event: FormEvent) {
        event.preventDefault();
        setError("");

        try {
            const response = await fetch("http://localhost:5207/api/Blog", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    title: title,
                    content: content,
                    category: category,
                    userId: Number(session.getId())
                })
            });

            if (!response.ok) {
                const message = await response.text();
                setError(message);
                return;
            }

            setTitle("");
            setContent("");
            setCategory("Recipes");

            await loadPosts();
        } catch {
            setError("Could not create the post. Please try again.");
        }
    }

    return (
        <div className="blog-page">
            <h1>Community Blog</h1>
            <p>Share your experiences, recipes and healthy habits with the Nivra community!</p>

            <h2>Create a post</h2>

            <form className="blog-form" onSubmit={handleSubmit}>
                <div>
                    <label>Title</label>
                    <input
                        type="text"
                        value={title}
                        onChange={event => setTitle(event.target.value)}
                        required
                    />
                </div>

                <div>
                    <label>Content</label>
                    <textarea
                        value={content}
                        onChange={event => setContent(event.target.value)}
                        required
                    />
                </div>

                <div>
                    <label>Category</label>
                    <select
                        value={category}
                        onChange={event => setCategory(event.target.value)}
                    >
                        <option value="Recipes">Recipes</option>
                        <option value="Fitness">Fitness</option>
                        <option value="Nutrition">Nutrition</option>
                        <option value="HealthyHabits">Healthy Habits</option>
                        <option value="PersonalExperience">Personal Experience</option>
                        <option value="Other">Other</option>
                    </select>
                </div>

                <button type="submit">Publish post</button>
            </form>

            {error && <p>{error}</p>}

            <div className="blog-posts">
            <h2>Latest posts</h2>

            {posts.map(post => (
                <div className="blog-post" key={post.id}>
                    <h3>{post.title}</h3>
                    <p>{post.content}</p>
                    <p className="blog-category">Category: {post.category}</p>
                    <p className="blog-post-info">By {post.username}</p>
                    <p className="blog-post-info">{new Date(post.createdAt).toLocaleDateString()}</p>
                </div>
            ))}  </div>
        </div>
    );
}
