export const getWallets = async (token) => {
    const response = await fetch('/api/wallets', {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });

    if (!response.ok) {
        if (response.status === 401) throw new Error('Unauthorized');
        throw new Error('Failed to fetch wallets');
    }

    return await response.json();
}