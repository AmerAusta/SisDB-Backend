using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Question
{
    public int QuestionId { get; set; }

    public int QuizId { get; set; }

    public string QuestionText { get; set; } = null!;

    public int Points { get; set; }

    public virtual ICollection<QuestionChoice> QuestionChoices { get; set; } = new List<QuestionChoice>();

    public virtual Quiz Quiz { get; set; } = null!;
}
