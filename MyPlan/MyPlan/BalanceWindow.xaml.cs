using System;
using System.Linq;
using System.Windows;
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

        private void SetInitialBalance_Click(object sender, RoutedEventArgs e)
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
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Montant invalide");
            }
        }

        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            var accueil = new Accueil();
            accueil.Show();
            this.Close();
        }

    }
}