using System.Windows;
using MyPlan.models;

public sealed class AppState
{
    private static readonly Lazy<AppState> _instance = new Lazy<AppState>(() => new AppState());
    public static AppState Instance => _instance.Value;

    private decimal _totalBalance;

    public decimal TotalBalance
    {
        get => _totalBalance;
        private set
        {
            if (_totalBalance != value)
            {
                _totalBalance = value;
                SaveToDatabase();
                BalanceChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler BalanceChanged;

    private AppState()
    {
        LoadFromDatabase();
    }

    public void LoadFromDatabase()
    {
        try
        {
            using var db = new BudgetContext();
            db.Database.EnsureCreated(); // Assure que la DB existe

            var balance = db.GlobalBalances.Find(1);
            if (balance == null)
            {
                balance = new GlobalBalance { Balance = 0 };
                db.GlobalBalances.Add(balance);
                db.SaveChanges();
            }
            _totalBalance = balance.Balance; // On assigne directement pour éviter la sauvegarde
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur de chargement du solde: {ex.Message}");
            _totalBalance = 0;
        }
    }

    private void SaveToDatabase()
    {
        try
        {
            using var db = new BudgetContext();
            var balance = db.GlobalBalances.Find(1);

            if (balance == null)
            {
                balance = new GlobalBalance { Balance = _totalBalance };
                db.GlobalBalances.Add(balance);
            }
            else
            {
                balance.Balance = _totalBalance;
                balance.LastUpdated = DateTime.Now;
            }

            db.SaveChanges();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erreur de sauvegarde du solde: {ex.Message}");
        }
    }

    public void SetInitialBalance(decimal amount)
    {
        if (amount < 0) throw new ArgumentException("Le solde ne peut pas être négatif");
        TotalBalance = amount;
    }

    public void Debit(decimal amount)
    {
        if (amount > TotalBalance)
            throw new InvalidOperationException("Solde insuffisant");
        TotalBalance -= amount;
    }

    public void Credit(decimal amount)
    {
        TotalBalance += amount;
    }
}