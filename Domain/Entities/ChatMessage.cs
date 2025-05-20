using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.Entities
{
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string Text { get; set; } = "";
        public DateTime SentAt { get; set; }
    }
}