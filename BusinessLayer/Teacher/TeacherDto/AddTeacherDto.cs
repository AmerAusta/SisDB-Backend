using BusinessLayer.User.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLayer.Teacher.TeacherDto
{
    public class AddTeacherDto : AddUserDto
    {
        public AddTeacherDto()
        {
            // يعني أستاذ (RoleId = 3)
            RoleId = 3;
        }

        [JsonIgnore]
        public override int RoleId { get; set; } = 3;

        public int SubjectId { get; set; }

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }
    }
}