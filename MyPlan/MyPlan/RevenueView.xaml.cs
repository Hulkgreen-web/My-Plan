// RevenueView.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;

namespace MyPlan
{
    public partial class RevenueView : Window
    {
        public List<Categorie> CategorieList { get; set; }
        private List<Transaction> RevenuList;

        public RevenueView()
        {
            InitializeComponent();
        }

        //Fonction appelée dès le chargement de la fenêtre
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerCategories();
            ChargerRevenus();
            DataContext = this; // Pour permettre le binding dans la DataGrid
        }

        //Fonction permettant de charger les catégories stockées en base de données et de les ajouter aux
        //listes déroulantes
        private void ChargerCategories()
        {
            using var db = new BudgetContext();
            CategorieList = db.Categories.OrderBy(c => c.Nom).ToList();

            CategorieFilterComboBox.Items.Clear();
            CategorieFilterComboBox.Items.Add("Tous");
            foreach (var category in CategorieList)
            {
                CategorieFilterComboBox.Items.Add(category);
            }
            CategorieFilterComboBox.SelectedIndex = 0;
        }

        //Fonction permettant de charger les revenus et de les ajouter au tableau 
        private void ChargerRevenus(int? categorieId = null)
        {
            using var db = new BudgetContext();
            var query = db.Transactions.Include(t => t.CategorieTransaction).Where(t => t.EstRevenu);

            if (categorieId.HasValue)
                query = query.Where(t => t.CategorieTransaction != null && t.CategorieTransaction.Id == categorieId);

            RevenuList = query.OrderByDescending(t => t.Date).ToList();
            ListeRevenus.ItemsSource = RevenuList;
        }

        //Fonction permettant d'éditer tous les champs d'un revenu 
        private void ListeRevenus_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Utiliser Dispatcher pour attendre la fin du commit
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
                            transactionExistante.Description = transaction.Description;
                            transactionExistante.Montant = transaction.Montant;
                            transactionExistante.Date = transaction.Date;
                            transactionExistante.EstRevenu = transaction.EstRevenu;

                            // Met à jour la catégorie
                            int? categorieId = transaction.CategorieTransaction?.Id;
                            if (categorieId.HasValue)
                            {
                                var categorie = db.Categories.FirstOrDefault(c => c.Id == categorieId.Value);
                                transactionExistante.CategorieTransaction = categorie;
                            }
                            else
                            {
                                transactionExistante.CategorieTransaction = null;
                            }

                            db.SaveChanges();
                            ChargerRevenus();
                        }
                    }
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        //Fonction qui définit l'enregistrement en base de données des modifications 
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

                    existing.CategorieTransaction = revenu.CategorieTransaction != null ?
                        db.Categories.Find(revenu.CategorieTransaction.Id) : null;
                }
            }

            db.SaveChanges();
            MessageBox.Show("Modifications enregistrées avec succès !");
        }

        //Fonction permettant de filtrer les revenus selon sa catégorie
        private void CategorieFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategorieFilterComboBox.SelectedItem is string && (string)CategorieFilterComboBox.SelectedItem == "Tous")
            {
                ChargerRevenus();
            }
            else if (CategorieFilterComboBox.SelectedItem is Categorie selectedCat)
            {
                ChargerRevenus(selectedCat.Id);
            }
        }

        //Fonction pour réinitialiser le filtre appliqué
        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CategorieFilterComboBox.SelectedIndex = 0;
        }

        //Fonction permettant de revenir au menu principal
        private void RetourAccueil_Click(object sender, RoutedEventArgs e)
        {
            var accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
