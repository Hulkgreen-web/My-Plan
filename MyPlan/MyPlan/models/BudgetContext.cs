using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlan.models
{
    using Microsoft.EntityFrameworkCore;

    public class BudgetContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Epargne> Epargnes { get; set; }
        public DbSet<GlobalBalance> GlobalBalances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=budget.db");
    }

}
