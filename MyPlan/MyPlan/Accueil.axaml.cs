using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace MyPlan
{
    public partial class Accueil : Window
    {
        public Accueil()
        {
            InitializeComponent();
        }

        private void StartBudget_Click(object? sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void OuvrirEpargnes_Click(object? sender, RoutedEventArgs e)
        {
            EpargneView epargneView = new EpargneView();
            epargneView.Show();
            this.Close();
        }

        private void OuvrirRevenus_Click(object? sender, RoutedEventArgs e)
        {
            RevenueView revenueView = new RevenueView();
            revenueView.Show();
            this.Close();
        }

        private void OuvrirDepenses_Click(object? sender, RoutedEventArgs e)
        {
            DepenseView depenseView = new DepenseView();
            depenseView.Show();
            this.Close();
        }

        private void ManageBalance_Click(object? sender, RoutedEventArgs e)
        {
            var balanceWindow = new BalanceWindow();
            balanceWindow.Show();
            this.Close();
        }

        private void Quit_Click(object? sender, RoutedEventArgs e)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
            else
            {
                this.Close();
            }
        }
    }
}
