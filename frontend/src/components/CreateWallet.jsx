import { useState } from 'react';
import { createWallet } from '../services/walletService';


{/*Wallet creation logic*/}
const CreateWallet = ({ token, onSuccess, onCancel }) => {
    const [name, setName] = useState('');
    const [currency, setCurrency] = useState('USD');
    const [balance, setBalance] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);

        const walletData = {
            Name: name,
            Currency: currency,
            Balance: balance ? parseFloat(balance) : 0
        };

        try {
            await createWallet(token, walletData);
            onSuccess();
        } catch (error) {
            console.error(error);
            alert('Failed to create wallet.');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div style={{ border: '1px solid #444', padding: '20px', marginBottom: '20px', borderRadius: '8px', background: '#2a2a2a' }}>
            <h3 style={{ color: 'white', marginTop: 0 }}>Create New Wallet</h3>
            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>

                <div>
                    <label style={{ color: '#ccc', display: 'block', marginBottom: '5px' }}>Wallet Name</label>
                    <input 
                        type="text" 
                        placeholder="e.g. Main Card"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                        style={{ padding: '8px', width: '100%', boxSizing: 'border-box' }}
                    />
                </div>

                <div>
                    <label style={{ color: '#ccc', display: 'block', marginBottom: '5px' }}>Initial Balance</label>
                    <input 
                        type="number" 
                        placeholder="0.00"
                        value={balance}
                        onChange={(e) => setBalance(e.target.value)}
                        step="0.01"
                        style={{ padding: '8px', width: '100%', boxSizing: 'border-box' }}
                    />
                </div>

                <div>
                    <label style={{ color: '#ccc', display: 'block', marginBottom: '5px' }}>Currency</label>
                    <select 
                        value={currency} 
                        onChange={(e) => setCurrency(e.target.value)}
                        style={{ padding: '8px', width: '100%' }}
                    >
                        <option value="USD">USD</option>
                        <option value="EUR">EUR</option>
                        <option value="PLN">PLN</option>
                    </select>
                </div>

                <div style={{ marginTop: '10px' }}>
                    <button type="submit" disabled={isSubmitting} style={{ padding: '8px 16px', cursor: 'pointer' }}>
                        {isSubmitting ? 'Saving...' : 'Create Wallet'}
                    </button>
                    <button 
                        type="button" 
                        onClick={onCancel} 
                        style={{ marginLeft: '10px', padding: '8px 16px', background: '#555', color: 'white', border: 'none', cursor: 'pointer' }}
                    >
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
};

export default CreateWallet;