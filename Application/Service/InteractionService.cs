using System;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class InteractionService
    {
        private readonly IInteractionRepository _repo;
        private readonly MatchService _matchService;


        public InteractionService(IInteractionRepository repo, MatchService matchService)
        {
            _repo = repo;
            _matchService = matchService;
        }

        /// <summary>
        /// Registra un "like" de un usuario hacia otro.
        /// </summary>
        /// <param name="userFrom">ID del usuario que envía el like.</param>
        /// <param name="userTo">ID del usuario que recibe el like.</param>
        public void Like(int userFrom, int userTo)
        {
            ValidateUsers(userFrom, userTo);
            try
            {
                // Graba la interacción
                _repo.Like(userFrom, userTo);

                // Comprueba reciprocidad: si userTo ya había dado like a userFrom
                if (_repo.HasInteraction(userTo, userFrom)
                    && !_matchService.AreMatched(userFrom, userTo))
                {
                    // Crea el match si aún no existe
                    _matchService.CreateMatch(userFrom, userTo);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al intentar dar like de {userFrom} a {userTo}: {ex.Message}", ex
                );
            }
        }

        /// <summary>
        /// Registra un "dislike" de un usuario hacia otro.
        /// </summary>
        /// <param name="userFrom">ID del usuario que envía el dislike.</param>
        /// <param name="userTo">ID del usuario que recibe el dislike.</param>
        public void Dislike(int userFrom, int userTo)
        {
            ValidateUsers(userFrom, userTo);

            try
            {
                _repo.Dislike(userFrom, userTo);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al intentar dar dislike de {userFrom} a {userTo}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validaciones comunes de IDs de usuario.
        /// </summary>
        private void ValidateUsers(int userFrom, int userTo)
        {
            if (userFrom <= 0)
                throw new ArgumentException("El ID del usuario emisor debe ser mayor que cero.", nameof(userFrom));
            if (userTo <= 0)
                throw new ArgumentException("El ID del usuario receptor debe ser mayor que cero.", nameof(userTo));
            if (userFrom == userTo)
                throw new ArgumentException("Un usuario no puede interactuar consigo mismo.");
        }
    }
}
