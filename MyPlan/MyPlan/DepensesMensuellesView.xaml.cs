using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;

namespace MyPlan
{
    public partial class DepensesMensuellesView : Window
    {
        public DepensesMensuellesView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChargerMoisEtAnnees();
            if (MoisComboBox.SelectedValue is int mois && AnneeComboBox.SelectedItem is int annee)
            {
                ChargerDepenses(mois, annee);
            }
        }

        private void ComboBoxDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MoisComboBox.SelectedValue is int mois && AnneeComboBox.SelectedItem is int annee)
            {
                ChargerDepenses(mois, annee);
            }
        }

        private void ChargerMoisEtAnnees()
        {
            // Mois (noms français)
            var moisNoms = CultureInfo.GetCultureInfo("fr-FR").DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m)) // Exclure la case vide à la fin
                .Select((nom, index) => new { Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nom), Numero = index + 1 })
                .ToList();

            MoisComboBox.ItemsSource = moisNoms;
            MoisComboBox.DisplayMemberPath = "Nom";
            MoisComboBox.SelectedValuePath = "Numero";

            // Années (exemple : de 2022 à l'année actuelle)
            int currentYear = DateTime.Now.Year;
            var annees = Enumerable.Range(currentYear - 3, 5).ToList();
            AnneeComboBox.ItemsSource = annees;

            // Pré-sélection mois et année actuels
            MoisComboBox.SelectedValue = DateTime.Now.Month;
            AnneeComboBox.SelectedItem = currentYear;
        }


        private void ChargerDepenses(int mois, int annee)
        {
            using var db = new BudgetContext();

            var transactions = db.Transactions
                .Include(t => t.CategorieTransaction)
                .AsEnumerable()
                .Where(t => !t.EstRevenu)
                .Where(t =>
                    DateTime.TryParseExact(t.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                    && parsedDate.Month == mois
                    && parsedDate.Year == annee)
                .ToList();

            var depensesParCategorie = transactions
                .GroupBy(t => t.CategorieTransaction?.Nom ?? "Sans catégorie")
                .Select(g => new
                {
                    Nom = g.Key,
                    MontantEstime = g.FirstOrDefault()?.CategorieTransaction?.montantEstime ?? 0,
                    MontantReel = g.Sum(t => t.Montant),
                    Ecart = (g.FirstOrDefault()?.CategorieTransaction?.montantEstime ?? 0) - g.Sum(t => t.Montant)
                })
                .ToList();

            DepensesParCategorieDataGrid.ItemsSource = depensesParCategorie;

            // ✅ Chargement du graphique
            DepensesPieChart.Series = new SeriesCollection();
            foreach (var cat in depensesParCategorie)
            {
                if (cat.MontantReel > 0)
                {
                    DepensesPieChart.Series.Add(new PieSeries
                    {
                        Title = cat.Nom,
                        Values = new ChartValues<decimal> { cat.MontantReel },
                        DataLabels = true
                    });
                }
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
