const API_URL = '/api';

export const getWallets = async (token) => {
    const response = await fetch(`${API_URL}/wallets`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!response.ok) throw new Error(response.statusText);
    return await response.json();
};

export const createWallet = async (token, walletData) => {
    const response = await fetch(`${API_URL}/wallets`, {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
        },
        body: JSON.stringify(walletData)
    });
    
    if (!response.ok) throw new Error('Wallet creation failure.');
    return await response.json();
};