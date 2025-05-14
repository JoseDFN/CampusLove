using CampusLove.Domain.DTO;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IAppUserRepository : IGenericRepository<DtoAppUser>
    {
        void update (int id, DtoAppUser entity);
    }
}