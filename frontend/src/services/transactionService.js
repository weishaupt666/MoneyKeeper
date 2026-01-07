const API_URL = '/api';

export const createTransaction = async (token, transactionData) => {
    const response = await fetch(`${API_URL}/transactions`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(transactionData)
    });

    if (!response.ok) {
        const errorText = await response.text();
        try {
            const jsonError = JSON.parse(errorText);
            console.error('Validation errors:', jsonError);
        } catch {
            console.error('Server error:', errorText);
        }
        throw new Error('Transaction creation failed');
    }

    return await response.json();
};