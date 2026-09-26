using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Attendance.AttendanceDto
{
    public class AddAttendanceDto
    {
        public int StudentId { get; set; }
        public int LessonId { get; set; }
         
    }
}