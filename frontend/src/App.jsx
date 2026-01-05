import { useEffect, useState } from 'react'
import Login from './components/Login'
import Register from './components/Register'
import { getWallets } from './services/walletService';
import './App.css'

function App() {
  const [wallets, setWallets] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isRegistering, setIsRegistering] = useState(false);
  
  const [token, setToken] = useState(localStorage.getItem('jwt_token'));

  const handleLogout = () => {
    localStorage.removeItem('jwt_token');
    setToken(null);
    setWallets([]);
  };

  useEffect(() => {
    if (token) {
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
    }
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
    <div style={{ padding: '20px' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h1>MoneyKeeper 💰</h1>
          <button onClick={handleLogout} style={{ background: '#ff4d4f' }}>Logout</button>
      </div>

      {isLoading && <p>Loading data...</p>}
      
      {!isLoading && wallets.length === 0 && (
          <p>No wallets found. Create your first one!</p>
      )}

      {wallets.map(wallet => (
        <li key={wallet.id}>
           {wallet.name}: {wallet.balance} {wallet.currencyCode}
        </li>
      ))}
    </div>
  )
}

export default App