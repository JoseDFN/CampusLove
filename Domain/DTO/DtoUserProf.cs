using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.DTO
{
    public class DtoUserProf
    {
        public int UserId { get; set; }
        public int PreferenceId { get; set; }
        public string? ProfileText { get; set; }
        public int AddressId { get; set; }
        public bool Verified { get; set; }
        public string? Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}