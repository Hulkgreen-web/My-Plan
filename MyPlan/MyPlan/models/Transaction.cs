﻿using System;
using MyPlan.models;

public class Transaction
{
    private static int nextId = 1;
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Description { get; set; }
    public decimal Montant { get; set; }
    public bool EstRevenu { get; set; } // true = revenu, false = dépense
    public int? CategorieId { get; set; }
    public Categorie? CategorieTransaction { get; set; }

    public Transaction(string description,decimal montant,bool estRevenu)
    {
        Description = description;
        Montant = montant;
        EstRevenu = estRevenu;
    }
}

