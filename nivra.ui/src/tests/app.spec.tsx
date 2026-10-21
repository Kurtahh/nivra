import { render, screen } from '@testing-library/react';
import App from '../App';

describe('App component', () => {
    it('renders the login form at the root route', () => {
        render(<App />);
        expect(screen.getByRole('heading', { name: /log in/i })).toBeInTheDocument();
    });
});