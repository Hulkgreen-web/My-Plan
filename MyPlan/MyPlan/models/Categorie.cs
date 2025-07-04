﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPlan.models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public decimal montantEstime { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public Categorie(string nom)
        {
            Nom = nom;
            Transactions = new List<Transaction>();
        }

        public void AddTransaction(Transaction transaction)
        {
            if (!Transactions.Contains(transaction))
            {
                Transactions.Add(transaction);
            }
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (Transactions.Contains(transaction))
            {
                Transactions.Remove(transaction);
            }
        }

    }
}
