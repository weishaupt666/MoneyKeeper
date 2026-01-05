import { useState } from 'react';
import { registerUser } from '../services/authService';

function Register({ onRegisterSuccess, onBackToLogin }) {
    const [formData, setFormData] = useState({
        username: '',
        password: '',
    });
    const [error, setError] = useState('');
    const [successMsg, setSuccessMsg] = useState('');

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            await registerUser(formData.username, formData.password);

            setSuccessMsg('Account created successfully! You can now login.');
            setTimeout(() => {
                onRegisterSuccess();
            }, 2000);
        } catch (err) {
            setError(err.message);
        }
    };

    return (
        <div style={{ maxWidth: '300px', margin: 'auto', padding: '20px', border: '1px solid #ccc', borderRadius: '8px' }}>
            <h2>Create Account</h2>

            {successMsg ? (
                <p style={{ color: 'green' }}>{successMsg}</p>
            ) : (
                <form onSubmit={handleSubmit}>
                    <div style={{ marginBottom: '10px' }}>
                        <label>Username:</label><br />
                        <input name="username" type="text" value={formData.username} onChange={handleChange} required />
                    </div>
                    <div style={{ marginBottom: '10px' }}>
                        <label>Password:</label><br />
                        <input name="password" type="password" value={formData.password} onChange={handleChange} required />
                    </div>

                    {error && <p style={{ color: 'red' }}>{error}</p>}

                    <button type="submit" style={{ width: '100%', marginBottom: '10px' }}>Sign Up</button>

                    <button type="button" onClick={onBackToLogin} style={{ width: '100%', background: '#ccc', color: 'black' }}>
                        Back to Login
                    </button>
                </form>
            )}
        </div>
    );
}

export default Register;