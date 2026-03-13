using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Domian.Entites
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string? First_name { get; set; }

        public string? Last_name { get; set; }
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public Role Role { get; set; }

        public ICollection<Response> Responses { get; set; } = new List<Response>();

        public string PasswordHash { get; set; } = "";
    }
}
