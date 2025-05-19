using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IMatchRepository : IGenericRepository<Entities.Match>
    {
        bool ExistsMatchBetween(int userAId, int userBId);
        void CreateMatch(int userAId, int userBId);
    }
}