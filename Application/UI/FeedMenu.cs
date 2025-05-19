using System;
using System.Collections.Generic;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Services;
using CampusLove.Application.Service;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;

namespace SGCI_app.application.UI
{
    public class FeedMenu : BaseMenu
    {
        private readonly FeedService _feedService;
        private readonly InteractionService _interactionService;
        private readonly int _currentUserId;

        public FeedMenu(int currentUserId) : base(showIntro: false)
        {
            _currentUserId = currentUserId;

            // Configura tu cadena de conexión
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";

            // Instanciar repositorios y servicios
            IAppUserRepository userRepo = new ImpAppUserRepository(connStr);
            IInteractionRepository interRepo = new ImpInteractionRepository(connStr);
            IMatchRepository matchRepo = new ImpMatchRepository(connStr);

            _feedService = new FeedService(userRepo);
            var matchService = new MatchService(matchRepo);
            _interactionService = new InteractionService(interRepo, matchService);
        }

        public override void ShowMenu()
        {
            ShowHeader("Tu Feed");

            var candidates = _feedService.BuildFeed(_currentUserId);
            if (candidates == null || candidates.Count == 0)
            {
                ShowInfoMessage("No hay usuarios para mostrar en tu feed.");
                return;
            }

            foreach (var user in candidates)
            {
                Console.Clear();
                ShowUserCard(user);

                Console.WriteLine("[L]ike  [D]islike  [S]alir");
                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.L)
                {
                    _interactionService.Like(_currentUserId, user.UserId);
                    ShowSuccessMessage($"Le diste like a {user.Name}.");
                }
                else if (key == ConsoleKey.D)
                {
                    _interactionService.Dislike(_currentUserId, user.UserId);
                    ShowErrorMessage($"Le diste dislike a {user.Name}.");
                }
                else if (key == ConsoleKey.S)
                {
                    break;
                }
            }

            ShowSummary();
        }

        private void ShowUserCard(DtoAppUser user)
        {
            ShowHeader($"Usuario: {user.Name}");
            Console.WriteLine($"Edad: {user.Age}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Ciudad (ID): {user.Address.CityId}");
            DrawSeparator();
            Console.WriteLine("Perfil:");
            Console.WriteLine(user.UserProfile.ProfileText);
            DrawSeparator();
            Console.WriteLine($"Intereses en común: {user.UserProfile.CommonInterestCount}");
            Console.WriteLine(user.UserProfile.Verified ? "✓ Verificado" : "✗ No verificado");
            Console.WriteLine();
        }

        private void ShowSummary()
        {
            Console.Clear();
            ShowHeader("Resumen de Interacciones");

            // Asumimos que el repositorio distingue like/dislike mediante HasInteracted y un método WasLiked
            Console.WriteLine("Resumen no implementado: consulta tu repositorio para generar estadísticas.");
            Console.WriteLine("Presione cualquier tecla para volver al menú principal...");
            Console.ReadKey();
        }
    }
}
