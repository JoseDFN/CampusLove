using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.infrastructure.postgres;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpChatMessageRepository : IChatMessageRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpChatMessageRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(ChatMessage msg)
        {
            var sql = @"
              INSERT INTO chat_message (match_id, sender_id, text)
              VALUES (@m, @s, @t);
            ";
            using var cmd = new NpgsqlCommand(sql, _conexion.ObtenerConexion());
            cmd.Parameters.AddWithValue("m", msg.MatchId);
            cmd.Parameters.AddWithValue("s", msg.SenderId);
            cmd.Parameters.AddWithValue("t", msg.Text);
            cmd.ExecuteNonQuery();
        }

        public List<ChatMessage> GetByMatch(int matchId)
        {
            var list = new List<ChatMessage>();
            var sql = @"
              SELECT message_id, match_id, sender_id, text, sent_at
              FROM chat_message
              WHERE match_id = @m;
            ";
            using var cmd = new NpgsqlCommand(sql, _conexion.ObtenerConexion());
            cmd.Parameters.AddWithValue("m", matchId);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new ChatMessage {
                    MessageId = rdr.GetInt32(0),
                    MatchId   = rdr.GetInt32(1),
                    SenderId  = rdr.GetInt32(2),
                    Text      = rdr.GetString(3),
                    SentAt    = rdr.GetDateTime(4)
                });
            }
            return list;
        }
    }
}