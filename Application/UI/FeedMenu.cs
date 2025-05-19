using System;
using System.Collections.Generic;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Services;
using CampusLove.Application.Service;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;
using Npgsql;

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
            var matchService = new MatchService(matchRepo, userRepo, connStr);
            _interactionService = new InteractionService(interRepo, matchService);
        }

        public override void ShowMenu()
        {
            ShowHeader("Tu Feed");

            var candidates = _feedService.BuildFeed(_currentUserId);
            if (candidates == null || candidates.Count == 0)
            {
                ShowInfoMessage("No hay usuarios para mostrar en tu feed.");
                Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
                Console.ReadLine();
                return;
            }

            // Obtener créditos disponibles
            int likesAvailable = GetAvailableLikes(_currentUserId);
            ShowInfoMessage($"Likes disponibles hoy: {likesAvailable}");

            foreach (var user in candidates)
            {
                Console.Clear();
                ShowUserCard(user);

                // Mostrar créditos disponibles en cada iteración
                Console.WriteLine($"\nLikes disponibles hoy: {likesAvailable}");
                Console.WriteLine("[L]ike  [D]islike  [S]alir");
                var key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.L)
                {
                    try
                    {
                        _interactionService.Like(_currentUserId, user.UserId);
                        likesAvailable--; // Decrementar contador local
                        ShowSuccessMessage($"Le diste like a {user.Name}. Te quedan {likesAvailable} likes para hoy.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        ShowErrorMessage($"No puedes dar like: {ex.Message}");
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                    }
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

        private int GetAvailableLikes(int userId)
        {
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            const string sql = @"
                SELECT likes_available 
                FROM interaction_credits 
                WHERE user_id = @userId 
                AND on_date = CURRENT_DATE;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userId", userId);
            
            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
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

            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            using var conn = new NpgsqlConnection(connStr);
            conn.Open();

            // 1. Obtener estadísticas de likes/dislikes del día
            const string dailyStatsSql = @"
                SELECT 
                    COUNT(CASE WHEN i.interaction_type_id = 1 THEN 1 END) as likes_given,
                    COUNT(CASE WHEN i.interaction_type_id = 2 THEN 1 END) as dislikes_given
                FROM interaction i
                WHERE i.source_user_id = @userId 
                AND DATE(i.created_at) = CURRENT_DATE;";

            int likesGiven = 0;
            int dislikesGiven = 0;
            using (var cmd = new NpgsqlCommand(dailyStatsSql, conn))
            {
                cmd.Parameters.AddWithValue("userId", _currentUserId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    likesGiven = reader.GetInt32(0);
                    dislikesGiven = reader.GetInt32(1);
                }
            }

            // 2. Obtener matches del día
            const string dailyMatchesSql = @"
                SELECT COUNT(*) 
                FROM match m
                WHERE (m.user1_id = @userId OR m.user2_id = @userId)
                AND DATE(m.matched_at) = CURRENT_DATE;";

            int matchesToday = 0;
            using (var cmd = new NpgsqlCommand(dailyMatchesSql, conn))
            {
                cmd.Parameters.AddWithValue("userId", _currentUserId);
                matchesToday = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 3. Obtener likes disponibles
            int likesAvailable = GetAvailableLikes(_currentUserId);

            // Mostrar resumen
            Console.WriteLine("\n=== Resumen del Día ===");
            Console.WriteLine($"Likes disponibles: {likesAvailable}");
            Console.WriteLine($"Likes dados hoy: {likesGiven}");
            Console.WriteLine($"Dislikes dados hoy: {dislikesGiven}");
            Console.WriteLine($"Matches generados hoy: {matchesToday}");

            // Calcular y mostrar tasa de match
            double matchRate = likesGiven > 0 ? (double)matchesToday / likesGiven * 100 : 0;
            Console.WriteLine($"\nTasa de match hoy: {matchRate:F2}%");

            // Mostrar mensaje motivacional basado en la tasa de match
            Console.WriteLine("\nEstado:");
            if (matchRate > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("¡Buen trabajo! Estás generando matches.");
            }
            else if (likesGiven > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Sigue intentando, los matches llegarán pronto.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Aún no has dado likes hoy. ¡Comienza a interactuar!");
            }
            Console.ResetColor();

            // Mostrar próximos pasos
            Console.WriteLine("\nPróximos pasos:");
            if (likesAvailable > 0)
            {
                Console.WriteLine($"• Aún tienes {likesAvailable} likes disponibles para hoy");
            }
            else
            {
                Console.WriteLine("• Has agotado tus likes por hoy. ¡Vuelve mañana!");
            }
            
            if (matchesToday > 0)
            {
                Console.WriteLine("• Revisa tus coincidencias en el menú principal");
            }

            Console.WriteLine("\nPresione cualquier tecla para volver al menú principal...");
            Console.ReadKey();
        }
    }
}
