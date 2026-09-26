using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Attendance.AttendanceDto
{
    public class UpdateAttendanceDto
    {
        public int AttendanceId { get; set; }
        public int? StudentId { get; set; }
        public int? LessonId { get; set; }
        public DateOnly? Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
