using BusinessLayer.User.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Student.StudentDto
{
    public class GetStudentDto:GetUserDto
    {
        public GetStudentDto() { }

        public int StudentId { get; set; }

        public int ClassId { get; set; }

        public int? ParentId { get; set; }

        public decimal TotalContractAmount { get; set; }

        public DateTime EnrollmentDate { get; set; }
    }
}
