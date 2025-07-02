using System;

public class Transaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Description { get; set; }
    public decimal Montant { get; set; }
    public bool EstRevenu { get; set; } // true = revenu, false = dépense
}

