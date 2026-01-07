import { useEffect, useState } from 'react'
import Login from './components/Login'
import Register from './components/Register'
import CreateWallet from './components/CreateWallet'
import { getWallets } from './services/walletService'
import CreateTransaction from './components/CreateTransaction'
import './App.css'

function App() {
  const [wallets, setWallets] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isRegistering, setIsRegistering] = useState(false);
  const [isCreating, setIsCreating] = useState(false);
  const [isCreatingTx, setIsCreatingTx] = useState(false);
  
  const [token, setToken] = useState(localStorage.getItem('jwt_token'));

  const handleLogout = () => {
    localStorage.removeItem('jwt_token');
    setToken(null);
    setWallets([]);
  };

  const handleWalletCreated = () => {
    setIsCreating(false);
    fetchWalletsData();
  };

  const handleTransactionCreated = () => {
    setIsCreatingTx(false);
    fetchWalletsData();
  }

  const fetchWalletsData = () => {
    if (!token) return;

    setIsLoading(true);
    getWallets(token)
    .then(data => {
      setWallets(data);
      setIsLoading(false);
    })
    .catch(err => {
      console.error(err);
      if (err.message === 'Unauthorized') {
        handleLogout();
      } else {
        setError(err.message);
      }
      setIsLoading(false);
    });
  };

  useEffect(() => {
    fetchWalletsData();
  }, [token]);
    
  if (!token) {
    if (isRegistering) {
      return (
        <Register
          onRegisterSuccess={() => setIsRegistering(false)}
          onBackToLogin={() => setIsRegistering(false)}
        />
      );
    }
    return (
      <Login 
        onLoginSuccess={(newToken) => setToken(newToken)}
        onRegisterClick={() => setIsRegistering(true)}
      />
    );
  }

return (
  <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <h1>MoneyKeeper 💰</h1>
        <button onClick={handleLogout} style={{ background: '#ff4d4f', color: 'white' }}>Logout</button>
    </div>

    <div style={{ marginBottom: '20px', display: 'flex', gap: '10px' }}>
      {!isCreating && !isCreatingTx && (
        <>
          <button onClick={() => setIsCreating(true)}>
            + New Wallet
          </button>
          <button onClick={() => setIsCreatingTx(true)} style={{ background: '#2196F3' }}>
            + New Transaction
          </button>
        </>
      )}
    </div>

    {isCreating && (
      <CreateWallet 
        token={token}
        onSuccess={handleWalletCreated}
        onCancel={() => setIsCreating(false)}
      />
    )}

    {isCreatingTx && (
      <CreateTransaction 
        token={token}
        wallets={wallets}
        onSuccess={handleTransactionCreated}
        onCancel={() => setIsCreatingTx(false)}
      />
    )}

    {isLoading && <p>Loading data...</p>}
    {error && <p style={{color: 'red'}}>{error}</p>}
      
    {!isLoading && wallets.length === 0 && !isCreating && (
      <p>No wallets found. Create your first one!</p>
    )}

    <ul style={{ listStyle: 'none', padding: 0 }}>
      {wallets.map(wallet => (
        <li key={wallet.id} style={{ 
          background: '#f5f5f5',
          color: '#333333',
          margin: '10px 0', 
          padding: '15px', 
          borderRadius: '8px',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}>
          <span style={{ fontWeight: 'bold', fontSize: '1.1rem' }}>{wallet.name}</span>
          <span style={{ fontWeight: 'bold', color: '#008000' }}>
            {wallet.balance.toFixed(2)} {wallet.currencyCode}
          </span>
        </li>
      ))}
    </ul>
  </div>
);
}

export default App