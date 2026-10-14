using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MyPlan.models;

namespace MyPlan
{
    public partial class CategorieEditView : Window
    {
        public CategorieEditView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            RafraichirListe();
        }

        private void RafraichirListe()
        {
            using var db = new BudgetContext();
            CategorieDataGrid.ItemsSource = db.Categories.ToList();
        }

        private async void Enregistrer_Click(object? sender, RoutedEventArgs e)
        {
            using var db = new BudgetContext();

            if (CategorieDataGrid.ItemsSource is IEnumerable<Categorie> categories)
            {
                foreach (var cat in categories)
                {
                    var catDb = db.Categories.Find(cat.Id);
                    if (catDb != null)
                    {
                        catDb.Nom = cat.Nom;
                        catDb.montantEstime = cat.montantEstime;
                    }
                }
            }

            db.SaveChanges();
            await DialogHelper.ShowMessageAsync(this, "Modifications enregistrées !");
            RafraichirListe();
        }

        private void Fermer_Click(object? sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
