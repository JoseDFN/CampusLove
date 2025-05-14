using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.DTO
{
    public class DtoAppUser
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int GenderId { get; set; }
        public int UserTypeId { get; set; }
        public DtoAddr Address { get; set; } = new DtoAddr();
        public DtoUserProf UserProfile { get; set; } = new DtoUserProf();
    }
}