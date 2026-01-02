namespace MoneyKeeper.Models;

public class Wallet
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public void Deposit(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Deposit amount must be positive");
        }

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount < 0) throw new ArgumentException("Withdraw amount must be positive");
        if (Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }
        Balance -= amount;
    }
}
