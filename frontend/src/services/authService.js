const API_URL = '/api';

export const loginUser = async (username, password) => {
    const response = await fetch(`${API_URL}/auth/login`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json' },
        body: JSON.stringify({username, password})
    });

    if (!response.ok) {
        throw new Error('Invalid credentails');
    }

    const data = await response.json();
    return data.accessToken || data.token;
};

export const registerUser = async (username, password) => {
    const response = await fetch(`${API_URL}/auth/register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    });

    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Registration failde');
    }

    return true;
}