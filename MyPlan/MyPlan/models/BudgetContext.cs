using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace MyPlan.models
{
    public class BudgetContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Categorie> Categories { get; set; } = null!;
        public DbSet<Epargne> Epargnes { get; set; } = null!;
        public DbSet<GlobalBalance> GlobalBalances { get; set; } = null!;

        public static string GetDatabasePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDir = Path.Combine(appData, "MyPlan");
            Directory.CreateDirectory(appDir);
            var targetDb = Path.Combine(appDir, "budget.db");

            // Migration automatique si une base existe à côté de l'exécutable
            if (!File.Exists(targetDb))
            {
                var localDb = Path.Combine(AppContext.BaseDirectory, "budget.db");
                if (File.Exists(localDb))
                {
                    File.Copy(localDb, targetDb);
                }
            }

            return targetDb;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={GetDatabasePath()}");
    }
}
