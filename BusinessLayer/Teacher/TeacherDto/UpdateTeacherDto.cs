using BusinessLayer.User.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Teacher.TeacherDto
{
    public class UpdateTeacherDto : UpdateUserDto
    {
        public UpdateTeacherDto() { }

        public int TeacherId { get; set; }

        public int? SubjectId { get; set; } = null;

        public decimal? Salary { get; set; } = null;

        public DateTime? HireDate { get; set; } = null;
    }
}