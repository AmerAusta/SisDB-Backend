using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public int UserId { get; set; }

    public int SubjectId { get; set; }

    public decimal Salary { get; set; }

    public DateTime HireDate { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
