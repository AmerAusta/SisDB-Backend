using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int StudentId { get; set; }

    public int InstallmentId { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime PaymentDate { get; set; }

    public int SecretaryId { get; set; }

    public virtual Installment Installment { get; set; } = null!;

    public virtual User Secretary { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
