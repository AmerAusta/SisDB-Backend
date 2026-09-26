using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.User.UserDto
{
    public class GetUserDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } 

        public string SecondName { get; set; } 

        public string LastName { get; set; } 

        public string Email { get; set; } 

        public string PhoneNumber { get; set; } 

        public int RoleId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
