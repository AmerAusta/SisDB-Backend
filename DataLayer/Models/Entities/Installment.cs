using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Installment
{
    public int InstallmentId { get; set; }

    public int StudentId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly DueDate { get; set; }

    public bool IsPaid { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Student Student { get; set; } = null!;
}
