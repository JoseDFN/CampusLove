using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class Match
    {
        public int MatchId { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime MatchedAt { get; set; }
    }
}