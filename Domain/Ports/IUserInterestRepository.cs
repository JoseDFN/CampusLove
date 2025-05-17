using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IUserInterestRepository : IGenericRepository<UserInterest>
    {
        public void Delete(int userId, int interestId);
        public void Update (int userId, int oldInterestId, int newInterestId);
    }
}