using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class Interaction
    {
        public int InteractionId { get; set; }
        public int SourceUserId { get; set; }
        public int TargetUserId { get; set; }
        public int InteractionTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}