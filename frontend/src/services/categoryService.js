const API_URL = '/api';
export const getCategories = async (token) => {
    const response = await fetch(`${API_URL}/categories`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!response.ok) throw new Error('Failed to fetch categories');
    return await response.json();
};