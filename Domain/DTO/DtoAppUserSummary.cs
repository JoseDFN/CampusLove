using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.DTO
{
    public class DtoAppUserSummary
    {
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Gender { get; set; } = "";
        public List<string> Careers { get; set; } = new();
        public List<string> Interests { get; set; } = new();
        public string City { get; set; } = "";
    }
}