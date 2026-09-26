using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Quiz
{
    public int QuizId { get; set; }

    public int LessonId { get; set; }

    public string Title { get; set; } = null!;

    public int DurationMinutes { get; set; }

    public int PassingScore { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
