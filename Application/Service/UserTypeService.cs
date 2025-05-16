using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class UserTypeService
    {
        private readonly IGenericRepository<UserType> _repo;

        public UserTypeService(IGenericRepository<UserType> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        public void CrearTipoUsuario(UserType usertype)
        {
            if (usertype == null)
                throw new ArgumentNullException(nameof(usertype));
            if (string.IsNullOrWhiteSpace(usertype.Description))
                throw new ArgumentException("El nombre del tipo de usuario no puede estar vacío.", nameof(usertype.Description));

            _repo.Create(usertype);
        }
        public void EliminarGenero(int usertypeId)
        {
            if (usertypeId <= 0)
                throw new ArgumentException("El ID del tipo de usuario debe ser mayor que cero.", nameof(usertypeId));

            _repo.Delete(usertypeId);
        }
        public void ActualizarGenero(UserType usertype)
        {
            if (usertype == null)
                throw new ArgumentNullException(nameof(usertype));
            if (usertype.Id <= 0)
                throw new ArgumentException("El ID del tipo de usuario debe ser mayor que cero.", nameof(usertype.Id));
            if (string.IsNullOrWhiteSpace(usertype.Description))
                throw new ArgumentException("El nombre del tipo de usuario no puede estar vacío.", nameof(usertype.Description));

            _repo.Update(usertype);
        }

        public List<UserType> ObtenerTodos()
        {
            return _repo.GetAll();
        }
    }
}