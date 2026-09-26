using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class StudentGrade
{
    public int GradeId { get; set; }

    public int StudentId { get; set; }

    public int QuizId { get; set; }

    public int Score { get; set; }

    public int AttemptNumber { get; set; }

    public DateTime DateAttempted { get; set; }

    public virtual Quiz Quiz { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
