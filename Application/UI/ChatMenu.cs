using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Application.UI;


namespace CampusLove.Application.UI
{
    public class ChatMenu : BaseMenu
    {
        private readonly ChatService _chatService;
        private readonly int _matchId, _meId;
        // Campo para controlar el flujo de ShowMenu (no concurrencia necesaria)

        public ChatMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpChatMessageRepository(connStr);
            _chatService = new ChatService(repo);
        }

        // Constructor con matchId y meId para inicializar la conversación de chat
        public ChatMenu(int matchId, int meId) : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpChatMessageRepository(connStr);
            _chatService = new ChatService(repo);
            _matchId = matchId;
            _meId = meId;
        }

        public override void ShowMenu()
        {
            Console.Clear();
            // Cargar historial inicial
            var history = _chatService.GetHistory(_matchId);
            int lastMessageId = history.Count > 0 ? history[^1].MessageId : 0;
            // Mostrar historial inicial
            foreach (var m in history)
            {
                var who = m.SenderId == _meId ? "Tú" : "Ellos";
                Console.WriteLine($"{m.SentAt:HH:mm} {who}: {m.Text}");
            }
            // Buffer de entrada manual
            var input = new System.Text.StringBuilder();
            Console.Write("\nEscribe tu mensaje (o 'salir'): ");
            while (true)
            {
                // Poll de nuevos mensajes si no hay tecla disponible
                if (!Console.KeyAvailable)
                {
                    System.Threading.Thread.Sleep(500);
                    var newMsgs = _chatService.GetHistory(_matchId)
                                    .Where(m => m.MessageId > lastMessageId)
                                    .ToList();
                    if (newMsgs.Count > 0)
                    {
                        foreach (var m in newMsgs)
                        {
                            var who = m.SenderId == _meId ? "Tú" : "Ellos";
                            Console.WriteLine($"\n{m.SentAt:HH:mm} {who}: {m.Text}");
                        }
                        lastMessageId = newMsgs.Last().MessageId;
                        // Reimprimir prompt e input pendiente
                        Console.Write("\nEscribe tu mensaje (o 'salir'): " + input);
                    }
                    continue;
                }
                var keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var text = input.ToString();
                    if (text.Equals("salir", StringComparison.OrdinalIgnoreCase)) break;
                    _chatService.Send(_matchId, _meId, text);
                    lastMessageId++;
                    Console.WriteLine($"\n{DateTime.Now:HH:mm} Tú: {text}");
                    input.Clear();
                    Console.Write("\nEscribe tu mensaje (o 'salir'): ");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input.Remove(input.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    input.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                }
            }
        }

        private void DrawHistory()
        {
            var history = _chatService.GetHistory(_matchId);
            foreach (var m in history)
            {
                var who = m.SenderId == _meId ? "Tú" : "Ellos";
                Console.WriteLine($"{m.SentAt:HH:mm} {who}: {m.Text}");
            }
        }
    }
}