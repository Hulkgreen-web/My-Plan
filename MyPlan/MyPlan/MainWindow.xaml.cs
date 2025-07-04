using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;

namespace MyPlan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DescriptionBox.TextChanged += (s, e) =>
            {
                PlaceholderDescription.Visibility = string.IsNullOrEmpty(DescriptionBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };

            MontantBox.TextChanged += (s, e) =>
            {
                PlaceholderMontant.Visibility = string.IsNullOrEmpty(MontantBox.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(MontantBox.Text, out decimal montant))
            {
                using var db = new BudgetContext();

                var transaction = new Transaction(DescriptionBox.Text, montant, EstRevenuCheck.IsChecked ?? false);

                if (CategorieComboBox.SelectedItem is Categorie selectedCategorie)
                {
                    // 🟢 Récupère la catégorie depuis le contexte actuel pour l’attacher correctement
                    var existingCategorie = db.Categories.Find(selectedCategorie.Id);
                    if (existingCategorie != null)
                    {
                        transaction.CategorieTransaction = existingCategorie;
                    }
                }

                db.Transactions.Add(transaction);
                db.SaveChanges();
            }

            DescriptionBox.Clear();
            MontantBox.Clear();
            EstRevenuCheck.IsChecked = false;

            ChargerTransactions();
        }


        private void ChargerTransactions()
        {
            using var db = new BudgetContext();

            // Inclure la catégorie liée
            var transactions = db.Transactions
                .Include(t => t.CategorieTransaction)
                .OrderByDescending(t => t.Date)
                .ToList();

            ListeTransactions.ItemsSource = transactions.Select(t => new
            {
                Transaction = t,
                Texte = $"{t.Date:dd/MM/yyyy} | {(t.EstRevenu ? "+" : "-")}{t.Montant}€ : {t.Description} " +
                        $"{(t.CategorieTransaction != null ? "| Catégorie : " + t.CategorieTransaction.Nom : "")}"
            }).ToList();

            ListeTransactions.DisplayMemberPath = "Texte";

            decimal solde = db.Transactions.Sum(t => t.EstRevenu ? t.Montant : -t.Montant);
            SoldeText.Text = $"Solde : {solde} €";
        }


        private void ChargerCategories()
        {
            using var db = new BudgetContext();
            var categories = db.Categories.OrderBy(c => c.Nom).ToList();
            CategorieComboBox.ItemsSource = categories;
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ListeTransactions.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Sélectionnez une transaction à supprimer.");
                return;
            }

            // Si tu as utilisé un objet anonyme pour l'affichage :
            var transaction = selectedItem.GetType().GetProperty("Transaction")?.GetValue(selectedItem) as Transaction;

            if (transaction != null)
            {
                using var db = new BudgetContext();
                var toDelete = db.Transactions.Find(transaction.Id);
                if (toDelete != null)
                {
                    db.Transactions.Remove(toDelete);
                    db.SaveChanges();
                }
                ChargerTransactions();
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerTransactions();
            ChargerCategories();
        }

        private void OpenMainMenu_Click(object sender, RoutedEventArgs e)
        {
            Accueil mainWindow = new Accueil();
            mainWindow.Show();
            this.Close();
        }

        private void AjouterCategorie_Click(object sender, RoutedEventArgs e)
        {
            var nom = Microsoft.VisualBasic.Interaction.InputBox("Nom de la nouvelle catégorie :", "Ajouter Catégorie", "");

            if (!string.IsNullOrWhiteSpace(nom))
            {
                using var db = new BudgetContext();
                if (!db.Categories.Any(c => c.Nom.ToLower() == nom.ToLower()))
                {
                    db.Categories.Add(new Categorie(nom));
                    db.SaveChanges();
                    ChargerCategories();
                }
                else
                {
                    MessageBox.Show("Cette catégorie existe déjà.");
                }
            }
        }


    }
}