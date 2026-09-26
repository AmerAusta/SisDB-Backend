using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Class
{
    public int ClassId { get; set; }

    public string ClassName { get; set; } = null!;

    public string Branch { get; set; } = null!;

    public string AcademicYear { get; set; } = null!;

    public int Capacity { get; set; }

    public int CurrentCapacity { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
