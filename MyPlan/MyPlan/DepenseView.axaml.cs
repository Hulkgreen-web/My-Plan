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
    public partial class DepenseView : Window
    {
        public List<Categorie> CategorieList { get; set; } = new();
        public List<Transaction> DepenseList { get; set; } = new();

        private static readonly string[] ChartColors = new[]
        {
            "#FFA726", "#FB8C00", "#FFB74D", "#FFCC80", "#FFE0B2", "#F57C00"
        };

        public DepenseView()
        {
            InitializeComponent();
        }

        public void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            ChargerCategories();
            ChargerDepenses();
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

        private void ChargerDepenses(int? categorieId = null)
        {
            using var db = new BudgetContext();
            var query = db.Transactions.Include(t => t.CategorieTransaction).Where(t => t.EstRevenu == false);

            if (categorieId.HasValue)
                query = query.Where(t => t.CategorieTransaction != null && t.CategorieTransaction.Id == categorieId);

            DepenseList = query.OrderByDescending(t => t.Date).ToList();
            ListeDepenses.ItemsSource = DepenseList;
            MettreAJourGraphique();
        }

        private void MettreAJourGraphique()
        {
            if (DepenseList == null || DepenseList.Count == 0)
            {
                PieChartDepenses.Series = Array.Empty<ISeries>();
                return;
            }

            var grouped = DepenseList
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

            PieChartDepenses.Series = seriesList;
        }

        private void ListeDepenses_RowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
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

                            AppState.Instance.Credit(ancienneValeur);
                            AppState.Instance.Debit(transaction.Montant);
                            db.SaveChanges();
                            ChargerDepenses();
                        }
                    }
                }, DispatcherPriority.Background);
            }
        }

        private async void Enregistrer_Click(object? sender, RoutedEventArgs e)
        {
            using var db = new BudgetContext();

            foreach (var depense in DepenseList)
            {
                var existing = db.Transactions.Include(t => t.CategorieTransaction).FirstOrDefault(t => t.Id == depense.Id);
                if (existing != null)
                {
                    existing.Description = depense.Description;
                    existing.Montant = depense.Montant;
                    existing.Date = depense.Date;
                    existing.EstRevenu = depense.EstRevenu;

                    existing.CategorieTransaction = depense.CategorieTransaction != null ?
                        db.Categories.Find(depense.CategorieTransaction.Id) : null;
                }
            }

            db.SaveChanges();
            await DialogHelper.ShowMessageAsync(this, "Modifications enregistrées avec succès !");
        }

        private void CategorieFilterComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (CategorieFilterComboBox.SelectedItem is string str && str == "Tous")
            {
                ChargerDepenses();
            }
            else if (CategorieFilterComboBox.SelectedItem is Categorie selectedCat)
            {
                ChargerDepenses(selectedCat.Id);
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
