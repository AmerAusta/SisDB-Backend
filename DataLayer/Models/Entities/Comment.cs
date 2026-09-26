using System;
using System.Collections.Generic;

namespace DataLayer.Models.Entities;

public partial class Comment
{
    public int CommentId { get; set; }

    public int LessonId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
