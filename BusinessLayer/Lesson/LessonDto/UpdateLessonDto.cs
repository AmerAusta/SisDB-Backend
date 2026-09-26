using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Lesson.LessonDto
{
    public class UpdateLessonDto
    {
        public int LessonId { get; set; }
        public int? AssignmentId { get; set; }
        public string? Title { get; set; }
        public string? VideoUrl { get; set; }
        public string? PdfUrl { get; set; }
        public bool? IsPublished { get; set; }
    }
}
