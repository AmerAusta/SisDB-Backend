using BusinessLayer.User.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLayer.Student.StudentDto
{
    public class AddStudentDto:AddUserDto
    {
        public AddStudentDto()
        {
            //يعني طالب 
            RoleId = 4;
        }
       
        [JsonIgnore]
        public override int RoleId { get; set; } = 4;

        public int ClassId { get; set; }

        public decimal TotalContractAmount { get; set; }

        public int? ParentId { get; set; } = null;

        public DateTime EnrollmentDate { get; set; }

        
    }
}
