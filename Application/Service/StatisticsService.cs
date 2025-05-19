using System;
using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.infrastructure.postgres;

namespace CampusLove.Application.Service
{
    public class StatisticsService
    {
        private readonly IInteractionRepository _interactionRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IAppUserRepository _userRepo;
        private readonly ConexionSingleton _conexion;

        public StatisticsService(
            IInteractionRepository interactionRepo,
            IMatchRepository matchRepo,
            IAppUserRepository userRepo,
            string connectionString)
        {
            _interactionRepo = interactionRepo;
            _matchRepo = matchRepo;
            _userRepo = userRepo;
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public (int totalLikes, int totalMatches, double matchRate) GetSystemStatistics()
        {
            var conn = _conexion.ObtenerConexion();
            
            // Get total likes
            const string likesSql = @"
                SELECT COUNT(*) 
                FROM interaction 
                WHERE interaction_type_id = 1;";
            
            int totalLikes;
            using (var cmd = new Npgsql.NpgsqlCommand(likesSql, conn))
            {
                totalLikes = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Get total matches
            const string matchesSql = @"
                SELECT COUNT(*) 
                FROM match;";
            
            int totalMatches;
            using (var cmd = new Npgsql.NpgsqlCommand(matchesSql, conn))
            {
                totalMatches = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Calculate match rate
            double matchRate = totalLikes > 0 ? (double)totalMatches / totalLikes * 100 : 0;

            return (totalLikes, totalMatches, matchRate);
        }

        public (int userLikes, int userMatches, double userMatchRate) GetUserStatistics(int userId)
        {
            var conn = _conexion.ObtenerConexion();
            
            // Get user's likes
            const string userLikesSql = @"
                SELECT COUNT(*) 
                FROM interaction 
                WHERE source_user_id = @userId 
                AND interaction_type_id = 1;";
            
            int userLikes;
            using (var cmd = new Npgsql.NpgsqlCommand(userLikesSql, conn))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                userLikes = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Get user's matches
            const string userMatchesSql = @"
                SELECT COUNT(*) 
                FROM match 
                WHERE user1_id = @userId 
                OR user2_id = @userId;";
            
            int userMatches;
            using (var cmd = new Npgsql.NpgsqlCommand(userMatchesSql, conn))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                userMatches = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // Calculate user's match rate
            double userMatchRate = userLikes > 0 ? (double)userMatches / userLikes * 100 : 0;

            return (userLikes, userMatches, userMatchRate);
        }
    }
} 