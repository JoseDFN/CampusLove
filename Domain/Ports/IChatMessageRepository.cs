using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IChatMessageRepository
    {
        List<ChatMessage> GetByMatch(int matchId);
        void Create(ChatMessage msg);
    }
}