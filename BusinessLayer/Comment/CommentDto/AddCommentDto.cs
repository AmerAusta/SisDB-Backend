using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Comment.CommentDto
{
    public class AddCommentDto
    {
        public int LessonId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }
}
