using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class UserCareerService
    {
        private readonly IUserCareerRepository _repo;

        public UserCareerService(IUserCareerRepository repo)
        {
            _repo = repo;
        }

        public void CrearUserCareer(UserCareer usercareer)
        {
            if (usercareer == null)
                throw new ArgumentNullException(nameof(usercareer));

            if (usercareer.UserId <= 0)
                throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(usercareer.UserId));
            if (usercareer.CareerId <= 0)
                throw new ArgumentException("El ID de carrera debe ser mayor que cero.", nameof(usercareer.CareerId));

            var entity = new UserCareer
            {
                UserId   = usercareer.UserId,
                CareerId = usercareer.CareerId
            };

            _repo.Create(entity);
        }

        public void EliminarUserCareer(int userId, int careerId)
        {
            if (userId   <= 0) throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userId));
            if (careerId <= 0) throw new ArgumentException("El ID de carrera debe ser mayor que cero.", nameof(careerId));

            _repo.Delete(userId, careerId);
        }

        public void ActualizarUserCareer(int userId, int oldCareerId, int newCareerId)
        {
            if (userId       <= 0) throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userId));
            if (oldCareerId  <= 0) throw new ArgumentException("El ID de carrera (antiguo) debe ser mayor que cero.", nameof(oldCareerId));
            if (newCareerId  <= 0) throw new ArgumentException("El ID de carrera (nuevo) debe ser mayor que cero.", nameof(newCareerId));

            _repo.Update(userId, oldCareerId, newCareerId);
        }

        public List<UserCareer> ObtenerTodos()
        {
            var entities = _repo.GetAll();
            var dtos = new List<UserCareer>(entities.Count);

            foreach (var e in entities)
            {
                dtos.Add(new UserCareer
                {
                    UserId   = e.UserId,
                    CareerId = e.CareerId
                });
            }

            return dtos;
        }
    }
}