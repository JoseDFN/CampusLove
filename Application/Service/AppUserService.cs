using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class AppUserService
    {
        private readonly IAppUserRepository _repo;

        public AppUserService(IAppUserRepository repo)
        {
            _repo = repo;
        }
        public int CrearUsuario(DtoAppUser dtoAppUser)
        {
            if (dtoAppUser == null)
                throw new ArgumentNullException(nameof(dtoAppUser));

            // Lógica de validación previa, si aplica

            int userId = _repo.create(dtoAppUser);

            // Aquí podrías agregar lógica post-creación, como insertar carreras, intereses, etc.

            return userId;
        }
        public void EliminarUsuario(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userId));

            _repo.Delete(userId);
        }
        public void ActualizarUsuario(int userId, DtoAppUser dtoAppUser)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userId));
            if (dtoAppUser == null)
                throw new ArgumentNullException(nameof(dtoAppUser));

            // Aquí podrías verificar que el usuario existe o aplicar más validaciones
            _repo.update(userId, dtoAppUser);
        }

        public DtoAppUser ObtenerUsuarioPorEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email no puede estar vacío.", nameof(email));

            return _repo.ObtenerUsuarioPorEmail(email);
        }

    }
}