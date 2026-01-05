import { useState } from 'react';
import { loginUser } from '../services/authService';

const Login = ({ onLoginSuccess, onRegisterClick }) => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const token = await loginUser(username, password);

            localStorage.setItem('jwt_token', token);
            onLoginSuccess(token);
        }
        catch (err) {
            setError(err.message);
        }
    };

    return (
        <div style={{ maxWidth: '300px', margin: 'auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h2>Sign In</h2>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column' }}>
                <div style={{ marginBottom: '10px' }}>
                    <label>Username:</label><br />
                    <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} required style={{ width: '100%', boxSizing: 'border-box' }} />
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <label>Password:</label><br />
                    <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required style={{ width: '100%', boxSizing: 'border-box' }} />
                </div>
                
                {error && <p style={{ color: 'red' }}>{error}</p>}
                
                <button 
                    type="submit" 
                    style={{ 
                        width: '100px',
                        display: 'block',
                        margin: '0 auto',
                        padding: '10px',
                        background: '#1a1919',
                        color: 'white',
                        cursor: 'pointer'
                    }}
                >
                    Login
                </button>
                
                <p style={{ textAlign: 'center', margin: '10px 0' }}>or</p>
                
                <button 
                    type="button" 
                    onClick={onRegisterClick} 
                    style={{ 
                        width: '100px',
                        display: 'block',
                        margin: '0 auto',
                        padding: '10px',
                        background: '#1a1919',
                        color: 'white',
                        cursor: 'pointer'
                    }}
                >
                    Register
                </button>
            </form>
        </div>
    );
};

export default Login;