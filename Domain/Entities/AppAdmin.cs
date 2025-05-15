using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class AppAdmin
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int GenderId { get; set; }
    }
}