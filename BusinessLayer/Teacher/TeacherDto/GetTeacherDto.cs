using BusinessLayer.User.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Teacher.TeacherDto
{
    public class GetTeacherDto : GetUserDto
    {
        public GetTeacherDto() { }

        public int TeacherId { get; set; }

        public int SubjectId { get; set; }

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }
    }
}