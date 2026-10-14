using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MyPlan.models;

namespace MyPlan
{
    public partial class EpargneView : Window
    {
        public EpargneView()
        {
            InitializeComponent();

            // Force le chargement initial
            AppState.Instance.LoadFromDatabase();

            // Abonnement aux changements
            AppState.Instance.BalanceChanged += (s, e) => UpdateBalanceDisplay();

            // Affichage initial
            UpdateBalanceDisplay();
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            RafraichirListe();
            UpdateBalanceDisplay();
        }

        private void UpdateBalanceDisplay()
        {
            Dispatcher.UIThread.Post(() =>
            {
                SoldeGlobalTextBlock.Text = $"Solde global: {AppState.Instance.TotalBalance:C}";
            });
        }

        private void RafraichirListe()
        {
            using var db = new BudgetContext();
            ListeEpargnes.ItemsSource = db.Epargnes.ToList();
        }

        private async void CreerModifier_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NomEpargneBox.Text) ||
                !decimal.TryParse(MontantEpargneBox.Text, out decimal montant) ||
                !decimal.TryParse(ObjectifEpargneBox.Text, out decimal objectif))
            {
                await DialogHelper.ShowMessageAsync(this, "Veuillez entrer un nom valide, un montant et un objectif numériques.");
                return;
            }

            try
            {
                using var db = new BudgetContext();
                var existante = db.Epargnes.FirstOrDefault(ep => ep.nom.ToLower() == NomEpargneBox.Text.ToLower());

                if (existante != null)
                {
                    // Pour une modification, on ajuste la différence
                    decimal difference = montant - existante.montant;
                    AppState.Instance.Debit(difference);
                    existante.montant = montant;
                    existante.Objectif = objectif;
                }
                else
                {
                    // Pour une nouvelle épargne, on débite le montant
                    AppState.Instance.Debit(montant);
                    db.Epargnes.Add(new Epargne(NomEpargneBox.Text, montant, objectif));
                }

                db.SaveChanges();
                RafraichirListe();
                NomEpargneBox.Clear();
                MontantEpargneBox.Clear();
                ObjectifEpargneBox.Clear();
            }
            catch (InvalidOperationException ex)
            {
                await DialogHelper.ShowMessageAsync(this, ex.Message, "Erreur");
            }
        }

        private async void AjouterArgent_Click(object? sender, RoutedEventArgs e)
        {
            if (ListeEpargnes.SelectedItem is Epargne selection)
            {
                string? input = await DialogHelper.ShowInputAsync(this, "Montant à ajouter :", "Ajouter à l'épargne", "0");

                if (decimal.TryParse(input, out decimal ajout) && ajout > 0)
                {
                    try
                    {
                        using var db = new BudgetContext();
                        var epargne = db.Epargnes.Find(selection.Id);
                        if (epargne != null)
                        {
                            AppState.Instance.Debit(ajout);
                            epargne.montant += ajout;
                            db.SaveChanges();
                            RafraichirListe();
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        await DialogHelper.ShowMessageAsync(this, ex.Message, "Erreur");
                    }
                }
                else
                {
                    await DialogHelper.ShowMessageAsync(this, "Montant invalide.");
                }
            }
            else
            {
                await DialogHelper.ShowMessageAsync(this, "Sélectionnez une épargne.");
            }
        }

        private async void Supprimer_Click(object? sender, RoutedEventArgs e)
        {
            if (ListeEpargnes.SelectedItem is Epargne selection)
            {
                if (await DialogHelper.ShowConfirmAsync(this, "Supprimer cette épargne ?", "Confirmation"))
                {
                    using var db = new BudgetContext();
                    var epargne = db.Epargnes.Find(selection.Id);
                    if (epargne != null)
                    {
                        // Remboursement du montant lors de la suppression
                        AppState.Instance.Credit(epargne.montant);
                        db.Epargnes.Remove(epargne);
                        db.SaveChanges();
                        RafraichirListe();
                    }
                }
            }
            else
            {
                await DialogHelper.ShowMessageAsync(this, "Sélectionnez une épargne à supprimer.");
            }
        }

        private void Fermer_Click(object? sender, RoutedEventArgs e)
        {
            Accueil accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
