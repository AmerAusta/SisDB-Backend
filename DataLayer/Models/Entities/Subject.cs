using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
