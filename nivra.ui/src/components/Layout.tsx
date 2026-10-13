import {Link, Outlet} from 'react-router-dom';

export default function Layout({routes}: {routes: [string, string][]}) {
    
    return (
        <div>
            <nav>
                <ul className="nav-bar">
                    {routes.map(([route, title]) => 
                        <li key={route}>
                            <Link to={route}>{title}</Link>
                        </li>
                    )}
                </ul>
            </nav>
            <Outlet/>
        </div>
    )
}