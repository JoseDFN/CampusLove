using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IUserCareerRepository : IGenericRepository<UserCareer>
    {
        public void Delete(int userId, int careerId);
        public void Update (int userId, int oldCareerId, int newCareerId);
    }
}