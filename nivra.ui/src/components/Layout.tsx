import {Link, Outlet} from 'react-router-dom';

export default function Layout({routes}: {routes: [string, string][]}) {
    
    return (
        <div className="app-shell">
            <nav className="site-nav">
                <ul className="nav-bar">
                    {routes.map(([route, title]) => 
                        <li key={route}>
                            <Link className="nav-link" to={route}>{title}</Link>
                        </li>
                    )}
                </ul>
            </nav>
            <main className="app-main">
                <Outlet/>
            </main>
        </div>
    )
}
