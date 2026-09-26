using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Lesson.LessonDto
{
    public class UpdateLessonWithFilesDto
    {
        public int LessonId { get; set; }
        public int? AssignmentId { get; set; }
        public string? Title { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IFormFile? PdfFile { get; set; }
        public bool? IsPublished { get; set; }
    }
}
