using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MyPlan.models;

namespace MyPlan
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Initialisation de la base SQLite (équivalent de OnStartup)
            using (var db = new BudgetContext())
            {
                db.Database.EnsureCreated();
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Dans WPF, la fermeture d'une fenêtre pour en afficher une autre ne quitte pas l'application
                desktop.ShutdownMode = ShutdownMode.OnLastWindowClose;
                desktop.MainWindow = new Accueil();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
