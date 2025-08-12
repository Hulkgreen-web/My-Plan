using System;
using System.Linq;
using System.Windows;
using MyPlan.models;

namespace MyPlan
{
    public partial class EpargneView : Window
    {
        public EpargneView()
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
            ListeEpargnes.ItemsSource = db.Epargnes.ToList();
        }

        private void CreerModifier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomEpargneBox.Text) ||
                !decimal.TryParse(MontantEpargneBox.Text, out decimal montant) ||
                !decimal.TryParse(ObjectifEpargneBox.Text, out decimal objectif))
            {
                MessageBox.Show("Veuillez entrer un nom valide, un montant et un objectif numériques.");
                return;
            }

            using var db = new BudgetContext();
            var existante = db.Epargnes.FirstOrDefault(ep => ep.nom.ToLower() == NomEpargneBox.Text.ToLower());

            if (existante != null)
            {
                existante.montant = montant;
                existante.Objectif = objectif;
            }
            else
            {
                db.Epargnes.Add(new Epargne(NomEpargneBox.Text, montant, objectif));
            }

            db.SaveChanges();
            RafraichirListe();
            NomEpargneBox.Clear();
            MontantEpargneBox.Clear();
            ObjectifEpargneBox.Clear();
        }

        private void AjouterArgent_Click(object sender, RoutedEventArgs e)
        {
            if (ListeEpargnes.SelectedItem is Epargne selection)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Montant à ajouter :", "Ajouter à l'épargne", "0");

                if (decimal.TryParse(input, out decimal ajout) && ajout > 0)
                {
                    using var db = new BudgetContext();
                    var epargne = db.Epargnes.Find(selection.Id);
                    if (epargne != null)
                    {
                        epargne.montant += ajout;
                        db.SaveChanges();
                        RafraichirListe();
                    }
                }
                else
                {
                    MessageBox.Show("Montant invalide.");
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez une épargne.");
            }
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            if (ListeEpargnes.SelectedItem is Epargne selection)
            {
                if (MessageBox.Show("Supprimer cette épargne ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using var db = new BudgetContext();
                    var epargne = db.Epargnes.Find(selection.Id);
                    if (epargne != null)
                    {
                        db.Epargnes.Remove(epargne);
                        db.SaveChanges();
                        RafraichirListe();
                    }
                }
            }
            else
            {
                MessageBox.Show("Sélectionnez une épargne à supprimer.");
            }
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            Accueil accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
