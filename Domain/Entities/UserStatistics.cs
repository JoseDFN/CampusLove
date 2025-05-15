using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class UserStatistics
    {
        public int UserId { get; set; }
        public int LikesGiven { get; set; }
        public int LikesReceived { get; set; }
        public int TotalMatches { get; set; }
        public DateTime LastInteractionAt { get; set; }
    }
}