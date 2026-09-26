using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Attendance.AttendanceDto
{
    public class GetAttendanceWithFullDetailsDto
    {

        public int AttendanceId { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;

        public int LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? PdfUrl { get; set; }
        public bool IsPublished { get; set; }
    }
}