using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int AssignmentId { get; set; }

    public string Title { get; set; } = null!;

    public string? VideoUrl { get; set; }

    public string? PdfUrl { get; set; }

    public bool IsPublished { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
