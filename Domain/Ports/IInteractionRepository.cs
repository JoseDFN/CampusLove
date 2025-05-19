using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IInteractionRepository : IGenericRepository<Interaction>
    {
        public void Like (int UserFrom, int UserTo);
        public void Dislike (int UserFrom, int UserTo);
        bool HasInteraction(int sourceUserId, int targetUserId);
    }
}