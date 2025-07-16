using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;

namespace MyPlan
{
    /// <summary>
    /// Logique d'interaction pour RevenueView.xaml
    /// </summary>
    public partial class RevenueView : Window

    {
        private List<Categorie> CategorieList;
        private ObservableCollection<Transaction> RevenuList;

        public RevenueView()
        {
            InitializeComponent();
            this.CategorieList = new List<Categorie>();
            this.RevenuList = new ObservableCollection<Transaction>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerCategories();
            ChargerRevenues();
        }

        private void ChargerCategories()
        {
            using var db = new BudgetContext();
            var categories = db.Categories.OrderBy(c => c.Nom).ToList();

            CategorieFilterComboBox.Items.Clear();
            CategorieFilterComboBox.Items.Add("Tous");
            foreach (var category in categories)
            {
                CategorieFilterComboBox.Items.Add(category);
            }

            CategorieFilterComboBox.SelectedIndex = 0;
        }

        private void ChargerRevenues(int? categorieId = null)
        {
            using var db = new BudgetContext();
            CategorieList = db.Categories.ToList();

            var query = db.Transactions
                .Include(t => t.CategorieTransaction)
                .Where(t => t.EstRevenu);

            if (categorieId.HasValue)
                query = query.Where(t => t.CategorieTransaction != null && t.CategorieTransaction.Id == categorieId.Value);

            RevenuList = new ObservableCollection<Transaction>(query.OrderByDescending(t => t.Date).ToList());

            ListeRevenus.ItemsSource = RevenuList;
            ListeRevenus.DataContext = new { CategorieList };
        }

        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            using var db = new BudgetContext();

            foreach (var revenu in RevenuList)
            {
                var existing = db.Transactions.Include(t => t.CategorieTransaction).FirstOrDefault(t => t.Id == revenu.Id);
                if (existing != null)
                {
                    existing.Description = revenu.Description;
                    existing.Montant = revenu.Montant;
                    existing.Date = revenu.Date;
                    existing.EstRevenu = revenu.EstRevenu;
                    existing.CategorieTransaction = revenu.CategorieTransaction != null
                        ? db.Categories.FirstOrDefault(c => c.Id == revenu.CategorieTransaction.Id)
                        : null;
                }
            }

            db.SaveChanges();
            MessageBox.Show("Modifications enregistrées avec succès !");
            ChargerRevenues();
        }

        private void ListeRevenus_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    if (e.Row.Item is Transaction transaction)
                    {
                        using var db = new BudgetContext();
                        var transactionExistante = db.Transactions
                            .Include(t => t.CategorieTransaction)
                            .FirstOrDefault(t => t.Id == transaction.Id);

                        if (transactionExistante != null)
                        {
                            // Mise à jour des propriétés de la transaction
                            transactionExistante.Description = transaction.Description;
                            transactionExistante.Montant = transaction.Montant;
                            transactionExistante.Date = transaction.Date;
                            transactionExistante.EstRevenu = true;

                            // Mise à jour de la catégorie
                            if (transaction.CategorieTransaction != null)
                            {
                                var catId = transaction.CategorieTransaction.Id;
                                var categorie = db.Categories.FirstOrDefault(c => c.Id == catId);
                                transactionExistante.CategorieTransaction = categorie;
                            }
                            else
                            {
                                transactionExistante.CategorieTransaction = null;
                            }

                            db.SaveChanges();
                            // Optionnel : tu peux recharger l'affichage ici
                            // ChargerRevenues();
                        }
                    }
                });
            }
        }


        private void CategorieFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategorieFilterComboBox.SelectedItem is string && (string)CategorieFilterComboBox.SelectedItem == "Tous")
            {
                ChargerRevenues();
            }
            else if (CategorieFilterComboBox.SelectedItem is Categorie selectedCat)
            {
                ChargerRevenues(selectedCat.Id);
            }
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CategorieFilterComboBox.SelectedIndex = 0;
        }

        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            var accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
