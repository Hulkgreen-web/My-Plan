using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.EntityFrameworkCore;
using MyPlan.models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace MyPlan
{
    public partial class RevenueView : Window
    {
        public List<Categorie> CategorieList { get; set; } = new();
        public List<Transaction> RevenuList { get; set; } = new();

        private static readonly string[] ChartColors = new[]
        {
            "#FFA726", "#FB8C00", "#FFB74D", "#FFCC80", "#FFE0B2", "#F57C00"
        };

        public RevenueView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            ChargerCategories();
            ChargerRevenus();
            MettreAJourGraphique();
            DataContext = this;
        }

        private void ChargerCategories()
        {
            using var db = new BudgetContext();
            CategorieList = db.Categories.OrderBy(c => c.Nom).ToList();

            var items = new List<object> { "Tous" };
            items.AddRange(CategorieList);

            CategorieFilterComboBox.ItemsSource = items;
            CategorieFilterComboBox.SelectedIndex = 0;
        }

        private void ChargerRevenus(int? categorieId = null)
        {
            using var db = new BudgetContext();
            var query = db.Transactions.Include(t => t.CategorieTransaction).Where(t => t.EstRevenu);

            if (categorieId.HasValue)
                query = query.Where(t => t.CategorieTransaction != null && t.CategorieTransaction.Id == categorieId);

            RevenuList = query.OrderByDescending(t => t.Date).ToList();
            ListeRevenus.ItemsSource = RevenuList;
            MettreAJourGraphique();
        }

        private void MettreAJourGraphique()
        {
            if (RevenuList == null || RevenuList.Count == 0)
            {
                PieChartRevenus.Series = Array.Empty<ISeries>();
                return;
            }

            var grouped = RevenuList
                .Where(r => r.CategorieTransaction != null)
                .GroupBy(r => r.CategorieTransaction!.Nom)
                .Select(g => new
                {
                    Categorie = g.Key,
                    MontantTotal = g.Sum(t => t.Montant)
                })
                .ToList();

            var seriesList = new List<ISeries>();
            int colorIdx = 0;

            foreach (var group in grouped)
            {
                var colorHex = ChartColors[colorIdx % ChartColors.Length];
                seriesList.Add(new PieSeries<double>
                {
                    Name = group.Categorie,
                    Values = new double[] { (double)group.MontantTotal },
                    Fill = new SolidColorPaint(SKColor.Parse(colorHex)),
                    DataLabelsPaint = new SolidColorPaint(SKColors.DarkSlateGray),
                    DataLabelsFormatter = p => $"{p.Coordinate.PrimaryValue:N2} €"
                });
                colorIdx++;
            }

            PieChartRevenus.Series = seriesList;
        }

        private void ListeRevenus_RowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (e.Row.DataContext is Transaction transaction)
                    {
                        using var db = new BudgetContext();
                        var transactionExistante = db.Transactions
                            .Include(t => t.CategorieTransaction)
                            .FirstOrDefault(t => t.Id == transaction.Id);

                        if (transactionExistante != null)
                        {
                            var ancienneValeur = transactionExistante.Montant;
                            transactionExistante.Description = transaction.Description;
                            transactionExistante.Montant = transaction.Montant;
                            transactionExistante.Date = transaction.Date;
                            transactionExistante.EstRevenu = transaction.EstRevenu;

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

                            AppState.Instance.Debit(ancienneValeur);
                            AppState.Instance.Credit(transaction.Montant);
                            db.SaveChanges();
                            ChargerRevenus();
                        }
                    }
                }, DispatcherPriority.Background);
            }
        }

        private async void Enregistrer_Click(object? sender, RoutedEventArgs e)
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
            await DialogHelper.ShowMessageAsync(this, "Modifications enregistrées avec succès !");
        }

        private void CategorieFilterComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CategorieFilterComboBox.SelectedItem is string str && str == "Tous")
            {
                ChargerRevenus();
            }
            else if (CategorieFilterComboBox.SelectedItem is Categorie selectedCat)
            {
                ChargerRevenus(selectedCat.Id);
            }
        }

        private void ResetFilter_Click(object? sender, RoutedEventArgs e)
        {
            CategorieFilterComboBox.SelectedIndex = 0;
        }

        private void RetourAccueil_Click(object? sender, RoutedEventArgs e)
        {
            var accueil = new Accueil();
            accueil.Show();
            this.Close();
        }
    }
}
