using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public int ClassId { get; set; }

    public int? ParentId { get; set; }

    public decimal TotalContractAmount { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Installment> Installments { get; set; } = new List<Installment>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

    public virtual User User { get; set; } = null!;
}
