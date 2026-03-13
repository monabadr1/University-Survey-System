using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class RegisterDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";

        public string? First_name { get; set; }

        public string? Last_name { get; set; }

        public Role Role { get; set; }
    }
}
