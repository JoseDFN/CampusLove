using System;
using System.Collections.Generic;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    /// <summary>
    /// Service para gestionar lógica de Matches.
    /// </summary>
    public class MatchService
    {
        private readonly IMatchRepository _matchRepo;

        public MatchService(IMatchRepository matchRepo)
        {
            _matchRepo = matchRepo ?? throw new ArgumentNullException(nameof(matchRepo));
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

        private void ValidateUsers(int userAId, int userBId)
        {
            if (userAId <= 0) throw new ArgumentException("El ID del usuario A debe ser mayor a cero.", nameof(userAId));
            if (userBId <= 0) throw new ArgumentException("El ID del usuario B debe ser mayor a cero.", nameof(userBId));
            if (userAId == userBId) throw new ArgumentException("Un usuario no puede hacer match consigo mismo.");
        }
    }
}
