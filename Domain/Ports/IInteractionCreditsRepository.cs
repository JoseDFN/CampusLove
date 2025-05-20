using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IInteractionCreditsRepository : IGenericRepository<InteractionCredits>
    {
        void ResetLikesAvailable();
        DateTime GetLastEventDate(string eventName);
        void UpdateEventDate(string eventName, DateTime date);
    }
}