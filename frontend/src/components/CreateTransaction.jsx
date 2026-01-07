import { useState, useEffect } from 'react';
import { getCategories } from '../services/categoryService';
import { createTransaction } from '../services/transactionService';

const CreateTransaction = ({ token, wallets, onSuccess, onCancel  }) => {
    const [amount, setAmount] = useState('');
    const [description, setDescription] = useState('');
    const [currency, setCurrency] = useState('PLN');
    const [type, setType] = useState(0);
    const [date, setDate] = useState(new Date().toISOString().split('T')[0]);

    const [selectedWalletId, setSelectedWalletId] = useState('');
    const [selectedCategoryId, setSelectedCategoryId] = useState('');

    const [categories, setCategories] = useState([]);
    const [loadingCategories, setLoadingCategories] = useState(true);
    const [isSubmitting, setIsSubmitting] = useState(false);

    useEffect(() => {
        getCategories(token)
        .then(data => {
            setCategories(data);
            if (data.length > 0) setSelectedCategoryId(data[0].id);
        })
        .catch(err => console.error('Error loading categories:', err))
        .finally(() => setLoadingCategories(false));

        if (wallets.length > 0) setSelectedWalletId(wallets[0].id);
    }, [token, wallets]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsSubmitting(true);

        const transactionData = {
            Amount: parseFloat(amount),
            Type: type,
            CurrencyCode: currency,
            Description: description,
            WalletId: parseInt(selectedWalletId),
            CategoryId: parseInt(selectedCategoryId),
            Date: date
        };

        console.log('Sending transaction:', transactionData);

        try {
            await createTransaction(token, transactionData);
            onSuccess();
        } catch (error) {
            alert('Failed to create transaction. Check console.');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div style={{ border: '1px solid #444', padding: '20px', marginBottom: '8px', background: '#2a2a2a', color: 'white' }}>
            <h3 style={{ marginTop: 0 }}>New Transaction</h3>

            <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>

                {/* Operation type and sum */}
                <div style={{ display: 'flex', gap: '10px' }}>
                    <select
                        value={type}
                        onChange={(e) => setType(parseInt(e.target.value))}
                        style={{ padding: '8px', flex: 1}}
                    >
                        <option value="0">Expense (-)</option>
                        <option value="1">Income (+)</option>
                    </select>

                    <input
                        type="number"
                        placeholde="Amount"
                        value={amount}
                        onChange={(e) => setAmount(e.target.value)}
                        step="0.01"
                        required
                        style={{ padding: '8px', flex: 2 }} 
                    />

                    <select
                        value={currency}
                        onChange={(e) => setCurrency(e.target.value)}
                        style={{ padding: '8px', witdth: '80px' }}
                    >
                        <option value="PLN">PLN</option>
                        <option value="EUR">EUR</option>
                        <option value="USD">USD</option>
                    </select>
                </div>

                {/* Wallet */}
                <div>
                    <label style={{ fontSize: '0.9rem', color: '#ccc' }}>Wallet</label>
                    <select 
                        value={selectedWalletId} 
                        onChange={(e) => setSelectedWalletId(e.target.value)}
                        required
                        style={{ padding: '8px', width: '100%' }}
                    >
                        {wallets.map(w => (
                            <option key={w.id} value={w.id}>
                                {w.name} ({w.balance} {w.currencyCode})
                            </option>
                        ))}
                    </select>
                </div>

                {/* Category */}
                <div>
                    <label style={{ fontSize: '0.9rem', color: '#ccc' }}>Category</label>
                    {loadingCategories ? (
                        <p style={{fontSize: '0.8rem'}}>Loading categories...</p>
                    ) : (
                        <select 
                            value={selectedCategoryId} 
                            onChange={(e) => setSelectedCategoryId(e.target.value)}
                            required
                            style={{ padding: '8px', width: '100%' }}
                        >
                            {categories.length === 0 && <option value="">No categories found</option>}
                            {categories.map(c => (
                                <option key={c.id} value={c.id}>{c.name}</option>
                            ))}
                        </select>
                    )}
                </div>

                {/* Date and description */}
                <input 
                    type="date" 
                    value={date}
                    onChange={(e) => setDate(e.target.value)}
                    required
                    style={{ padding: '8px' }}
                />

                <input 
                    type="text" 
                    placeholder="Description (optional)"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    style={{ padding: '8px' }}
                />

                {/* Buttons */}
                <div style={{ marginTop: '10px' }}>
                    <button type="submit" disabled={isSubmitting} style={{ padding: '8px 16px', cursor: 'pointer', background: type === 'Income' ? '#4caf50' : '#f44336', color: 'white', border: 'none' }}>
                        {isSubmitting ? 'Processing...' : 'Add Transaction'}
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
    )
};

export default CreateTransaction;