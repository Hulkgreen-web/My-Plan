using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MyPlan
{
    public class MoisItem
    {
        public string Nom { get; set; } = string.Empty;
        public int Numero { get; set; }
        public override string ToString() => Nom;
    }

    public partial class RevenuMensuelView : Window
    {
        private static readonly string[] ChartColors = new[]
        {
            "#FFA726", "#FB8C00", "#FFB74D", "#FFCC80", "#FFE0B2", "#F57C00"
        };

        public RevenuMensuelView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            ChargerMoisEtAnnees();
            if (MoisComboBox.SelectedItem is MoisItem mois && AnneeComboBox.SelectedItem is int annee)
            {
                ChargerRevenus(mois.Numero, annee);
            }
        }

        private void ComboBoxDate_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (MoisComboBox.SelectedItem is MoisItem mois && AnneeComboBox.SelectedItem is int annee)
            {
                ChargerRevenus(mois.Numero, annee);
            }
        }

        private void ChargerMoisEtAnnees()
        {
            var moisNoms = CultureInfo.GetCultureInfo("fr-FR").DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .Select((nom, index) => new MoisItem { Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nom), Numero = index + 1 })
                .ToList();

            MoisComboBox.ItemsSource = moisNoms;

            int currentYear = DateTime.Now.Year;
            var annees = Enumerable.Range(currentYear - 3, 80).ToList();
            AnneeComboBox.ItemsSource = annees;

            MoisComboBox.SelectedItem = moisNoms.FirstOrDefault(m => m.Numero == DateTime.Now.Month);
            AnneeComboBox.SelectedItem = currentYear;
        }

        private void ChargerRevenus(int mois, int annee)
        {
            using var db = new BudgetContext();

            var transactions = db.Transactions
                .Include(t => t.CategorieTransaction)
                .AsEnumerable()
                .Where(t => t.EstRevenu)
                .Where(t =>
                    DateTime.TryParseExact(t.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate)
                    && parsedDate.Month == mois
                    && parsedDate.Year == annee)
                .ToList();

            var revenusParCategorie = transactions
                .GroupBy(t => t.CategorieTransaction?.Nom ?? "Sans catégorie")
                .Select(g => new
                {
                    Nom = g.Key,
                    MontantEstime = g.FirstOrDefault()?.CategorieTransaction?.montantEstime ?? 0,
                    MontantReel = g.Sum(t => t.Montant),
                    Ecart = (g.FirstOrDefault()?.CategorieTransaction?.montantEstime ?? 0) - g.Sum(t => t.Montant)
                })
                .ToList();

            RevenusParCategorieDataGrid.ItemsSource = revenusParCategorie;

            var series = new List<ISeries>();
            int idx = 0;

            foreach (var cat in revenusParCategorie)
            {
                if (cat.MontantReel > 0)
                {
                    var colorHex = ChartColors[idx % ChartColors.Length];
                    series.Add(new PieSeries<double>
                    {
                        Name = cat.Nom,
                        Values = new double[] { (double)cat.MontantReel },
                        Fill = new SolidColorPaint(SKColor.Parse(colorHex)),
                        DataLabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                        DataLabelsFormatter = p => $"{p.Coordinate.PrimaryValue:N2} €"
                    });
                    idx++;
                }
            }

            RevenusPieChart.Series = series;
        }

        private void RetourAccueil_Click(object? sender, RoutedEventArgs e)
        {
            var accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
