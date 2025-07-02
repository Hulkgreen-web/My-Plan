using System.Configuration;
using System.Data;
using System.Windows;
using MyPlan.models;

namespace MyPlan
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using var db = new BudgetContext();
            db.Database.EnsureCreated();
        }

    }

}
