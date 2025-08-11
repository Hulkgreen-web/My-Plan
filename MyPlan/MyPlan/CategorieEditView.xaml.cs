using System;
using System.Linq;
using System.Windows;
using MyPlan.models;

namespace MyPlan
{
    public partial class CategorieEditView : Window
    {
        public CategorieEditView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RafraichirListe();
        }

        private void RafraichirListe()
        {
            using var db = new BudgetContext();
            CategorieDataGrid.ItemsSource = db.Categories.ToList();
        }

        private void Enregistrer_Click(object sender, RoutedEventArgs e)
        {
            using var db = new BudgetContext();

            foreach (var item in CategorieDataGrid.Items)
            {
                if (item is Categorie cat)
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
            MessageBox.Show("Modifications enregistrées !");
            RafraichirListe();
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
