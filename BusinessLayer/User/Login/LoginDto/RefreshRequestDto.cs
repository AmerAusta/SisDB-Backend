using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.User.Login.LoginDto
{
    public class RefreshRequestDto
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
