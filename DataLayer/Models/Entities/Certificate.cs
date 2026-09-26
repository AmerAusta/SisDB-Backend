using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Certificate
{
    public int CertificateId { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public string Grade { get; set; } = null!;

    public virtual Class Class { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
