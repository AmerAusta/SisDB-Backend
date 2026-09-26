using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.User.UserDto
{
    public class UpdateUserDto
    {
        public int UserID { get; set; }

        public string? FirstName { get; set; }=null;

        public string? SecondName { get; set; } = null;

        public string? LastName { get; set; } = null;

        public string? Email { get; set; } = null;

        public string? Password { get; set; } = null;

        public string? PhoneNumber { get; set; } = null;

        public bool? IsActive { get; set; } =true;
    }
}
