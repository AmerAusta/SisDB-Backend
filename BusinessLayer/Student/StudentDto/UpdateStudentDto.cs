using BusinessLayer.User.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Student.StudentDto
{
    public class UpdateStudentDto:UpdateUserDto
    {
        public UpdateStudentDto() { }

        public int StudentId { get; set; }

        public int? ClassId { get; set; }=null;

        public int? ParentId { get; set; } = null;

    }
}
