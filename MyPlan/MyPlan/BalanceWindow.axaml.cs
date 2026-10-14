using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;

namespace MyPlan
{
    public partial class BalanceWindow : Window
    {
        public BalanceWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using var db = new BudgetContext();
            var balance = db.GlobalBalances
                          .FirstOrDefault(b => b.Id == 1);

            if (balance == null)
            {
                CurrentBalanceText.Text = "Non défini";
                return;
            }

            CurrentBalanceText.Text = balance.Balance.ToString("C");
        }

        private async void SetInitialBalance_Click(object? sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                try
                {
                    AppState.Instance.SetInitialBalance(amount);
                    LoadData();
                }
                catch (Exception ex)
                {
                    await DialogHelper.ShowMessageAsync(this, ex.Message, "Erreur");
                }
            }
            else
            {
                await DialogHelper.ShowMessageAsync(this, "Montant invalide", "Erreur");
            }
        }

        private void AmountTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SetInitialBalance_Click(sender, new RoutedEventArgs());
            }
        }

        private void RetourAccueil_Click(object? sender, RoutedEventArgs e)
        {
            var accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
