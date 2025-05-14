using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class InteractionCredits
    {
        public int CreditId { get; set; }
        public int UserId { get; set; }
        public DateTime OnDate { get; set; }
        public int LikesAvailable { get; set; }
    }
}