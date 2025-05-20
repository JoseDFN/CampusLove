using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Ports;
using Newtonsoft.Json;
using CampusLove.Domain.Entities;

namespace CampusLove.Application.Service
{
    public class ChatService
    {
        private readonly IChatMessageRepository _repo;

        public ChatService(IChatMessageRepository repo)
        {
            _repo = repo;
        }

        // LINQ para ordenar cronológicamente
        public List<ChatMessage> GetHistory(int matchId)
            => _repo.GetByMatch(matchId)
                    .OrderBy(m => m.SentAt)
                    .ToList();

        public void Send(int matchId, int senderId, string text)
        {
            var msg = new ChatMessage {
                MatchId  = matchId,
                SenderId = senderId,
                Text     = text
            };
            _repo.Create(msg);
        }

        // Métodos JSON usando Newtonsoft.Json
        public string GetHistoryJson(int matchId)
            => JsonConvert.SerializeObject(GetHistory(matchId));

        public void SendFromJson(string json)
        {
            var msg = JsonConvert.DeserializeObject<ChatMessage>(json)
                      ?? throw new System.Exception("JSON inválido");
            Send(msg.MatchId, msg.SenderId, msg.Text);
        }
    }
}