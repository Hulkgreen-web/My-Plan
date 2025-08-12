using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyPlan.models
{
    public class Epargne
    {
        public int Id { get; set; }
        public string nom {  get; set; }
        public decimal montant { get; set; }
        public decimal Objectif { get; set; }

        public Epargne(string nom, decimal montant,decimal objectif)
        {
            this.nom = nom;
            this.montant = montant;
            this.Objectif = objectif;
        }

        public double ProgressionPourcent
        {
            get
            {
                if (Objectif <= 0) return 0;
                return (double)montant / (double)Objectif;
            }
        }
    }
}
