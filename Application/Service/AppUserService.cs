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
        public void CrearUsuario(DtoAppUser dtoAppUser)
        {
            if (dtoAppUser == null)
                throw new ArgumentNullException(nameof(dtoAppUser));

            // Aquí podrías agregar lógica de validación adicional antes de llamar al repositorio
            _repo.Create(dtoAppUser);
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

    }
}