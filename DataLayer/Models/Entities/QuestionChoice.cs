using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class QuestionChoice
{
    public int ChoiceId { get; set; }

    public int QuestionId { get; set; }

    public string ChoiceText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual Question Question { get; set; } = null!;
}
