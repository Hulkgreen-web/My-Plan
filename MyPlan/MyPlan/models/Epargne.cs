using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlan.models
{
    public class Epargne
    {
        public int Id { get; set; }
        public string nom {  get; set; }
        public decimal montant { get; set; }

        public Epargne(string nom, decimal montant)
        {
            this.nom = nom;
            this.montant = montant;
        }
    }
}
