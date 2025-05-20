using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class InteractionCreditsService
    {
        private const string EventName = "ResetLikesDaily";
        private readonly IInteractionCreditsRepository _creditsRepo;

        public InteractionCreditsService(IInteractionCreditsRepository creditsRepo)
        {
            _creditsRepo = creditsRepo;
        }

        public void EnsureDailyReset()
        {
            var today = DateTime.UtcNow.Date;
            var lastRun = _creditsRepo.GetLastEventDate(EventName);

            if (lastRun < today)
            {
                // 1) Resetear credits
                _creditsRepo.ResetLikesAvailable();

                // 2) Registrar que hoy ya corrimos el evento
                _creditsRepo.UpdateEventDate(EventName, today);
            }
        }

        /// <summary>
        /// Fuerza un reset de likes_available sin lógica de fecha
        /// (útil solo para pruebas o llamadas especiales).
        /// </summary>
        public void ResetLikesAvailable()
        {
            try
            {
                _creditsRepo.ResetLikesAvailable();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al resetear likes_available: {ex.Message}", ex
                );
            }
        }
    }
}