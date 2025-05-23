using CampusLove.Domain.DTO;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IAppUserRepository : IGenericRepository<DtoAppUser>
    {
        void update(int id, DtoAppUser entity);
        int create(DtoAppUser entity);
        DtoAppUser ObtenerUsuarioPorEmail(string email);
        List<DtoAppUser> GetFeedCandidates(int currentUserId);
    }
}