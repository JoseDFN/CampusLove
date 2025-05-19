using System;
using System.Collections.Generic;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using CampusLove.Domain.DTO;
using SGCI_app.infrastructure.postgres;

namespace CampusLove.Application.Service
{
    /// <summary>
    /// Service para gestionar lógica de Matches.
    /// </summary>
    public class MatchService
    {
        private readonly IMatchRepository _matchRepo;
        private readonly IAppUserRepository _userRepo;
        private readonly ConexionSingleton _conexion;

        public MatchService(IMatchRepository matchRepo, IAppUserRepository userRepo, string connectionString)
        {
            _matchRepo = matchRepo ?? throw new ArgumentNullException(nameof(matchRepo));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        /// <summary>
        /// Crea un match entre dos usuarios si no existe.
        /// </summary>
        public void CreateMatch(int userAId, int userBId)
        {
            ValidateUsers(userAId, userBId);
            if (!_matchRepo.ExistsMatchBetween(userAId, userBId))
            {
                _matchRepo.CreateMatch(userAId, userBId);
            }
        }

        /// <summary>
        /// Recupera todos los matches existentes.
        /// </summary>
        /// <returns>Lista de objetos Match.</returns>
        public List<Match> GetAllMatches()
        {
            return _matchRepo.GetAll();
        }

        /// <summary>
        /// Verifica si dos usuarios ya tienen un match.
        /// </summary>
        public bool AreMatched(int userAId, int userBId)
        {
            ValidateUsers(userAId, userBId);
            return _matchRepo.ExistsMatchBetween(userAId, userBId);
        }

        /// <summary>
        /// Obtiene los matches de un usuario con detalles de los usuarios con los que hizo match.
        /// </summary>
        public List<(Match Match, DtoAppUser MatchedUser)> GetUserMatches(int userId)
        {
            var conn = _conexion.ObtenerConexion();
            var matches = new List<(Match, DtoAppUser)>();

            const string sql = @"
                SELECT 
                    m.match_id,
                    m.user1_id,
                    m.user2_id,
                    m.matched_at,
                    u.user_id,
                    u.name,
                    u.age,
                    g.description as gender,
                    c.name as city,
                    up.profile_text,
                    up.verified,
                    COUNT(ui.interest_id) as common_interests,
                    array_agg(i.description) as common_interest_names
                FROM match m
                JOIN app_user u ON (
                    CASE 
                        WHEN m.user1_id = @userId THEN m.user2_id = u.user_id
                        ELSE m.user1_id = u.user_id
                    END
                )
                JOIN user_profile up ON u.user_id = up.user_id
                JOIN gender g ON u.gender_id = g.gender_id
                JOIN address a ON up.address_id = a.id
                JOIN city c ON a.city_id = c.id
                LEFT JOIN user_interest ui ON ui.user_id = u.user_id
                LEFT JOIN user_interest my_ui ON my_ui.user_id = @userId
                LEFT JOIN interest i ON ui.interest_id = i.interest_id
                WHERE (m.user1_id = @userId OR m.user2_id = @userId)
                AND ui.interest_id = my_ui.interest_id
                GROUP BY 
                    m.match_id, m.user1_id, m.user2_id, m.matched_at,
                    u.user_id, u.name, u.age, g.description, c.name,
                    up.profile_text, up.verified
                ORDER BY m.matched_at DESC;";

            using var cmd = new Npgsql.NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var match = new Match
                {
                    MatchId = reader.GetInt32(0),
                    User1Id = reader.GetInt32(1),
                    User2Id = reader.GetInt32(2),
                    MatchedAt = reader.GetDateTime(3)
                };

                var matchedUser = new DtoAppUser
                {
                    UserId = reader.GetInt32(4),
                    Name = reader.GetString(5),
                    Age = reader.GetInt32(6),
                    UserProfile = new DtoUserProf
                    {
                        ProfileText = reader.IsDBNull(9) ? null : reader.GetString(9),
                        Verified = reader.GetBoolean(10),
                        CommonInterestCount = reader.GetInt32(11),
                        CommonInterestNames = reader.IsDBNull(12) ? new string[0] : (string[])reader.GetValue(12)
                    }
                };

                matches.Add((match, matchedUser));
            }

            return matches;
        }

        private void ValidateUsers(int userAId, int userBId)
        {
            if (userAId <= 0) throw new ArgumentException("El ID del usuario A debe ser mayor a cero.", nameof(userAId));
            if (userBId <= 0) throw new ArgumentException("El ID del usuario B debe ser mayor a cero.", nameof(userBId));
            if (userAId == userBId) throw new ArgumentException("Un usuario no puede hacer match consigo mismo.");
        }
    }
}
