using System;
using System.Threading;
using System.Threading.Tasks;
using CampusLove.Application.Service;

namespace CampusLove.Infrastructure.Scheduling
{
    public class DailyInteractionCreditsScheduler
    {
        private readonly InteractionCreditsService _creditsService;

        public DailyInteractionCreditsScheduler(InteractionCreditsService creditsService)
        {
            _creditsService = creditsService;
        }

        public void Start(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var now = DateTime.UtcNow;
                    // Calcula el próximo arranque a medianoche UTC
                    var nextMidnight = now.Date.AddDays(1);
                    var delay = nextMidnight - now;

                    await Task.Delay(delay, token);
                    if (token.IsCancellationRequested) break;

                    // Llama al servicio de aplicación
                    _creditsService.ResetLikesAvailable();

                    // Después de la primera ejecución, espera 24 h exactas
                    await Task.Delay(TimeSpan.FromHours(24), token);
                }
            }, token);
        }
    }
}
