using System;
using System.Collections.Generic;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class UserInterestService
    {
        private readonly IUserInterestRepository _repo;

        public UserInterestService(IUserInterestRepository repo)
        {
            _repo = repo;
        }

        public void CrearUserInterest(UserInterest userInterest)
        {
            if (userInterest == null)
                throw new ArgumentNullException(nameof(userInterest));

            if (userInterest.UserId <= 0)
                throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userInterest.UserId));
            if (userInterest.InterestId <= 0)
                throw new ArgumentException("El ID de interés debe ser mayor que cero.", nameof(userInterest.InterestId));

            var entity = new UserInterest
            {
                UserId = userInterest.UserId,
                InterestId = userInterest.InterestId
            };

            _repo.Create(entity);
        }

        public void EliminarUserInterest(int userId, int interestId)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userId));
            if (interestId <= 0)
                throw new ArgumentException("El ID de interés debe ser mayor que cero.", nameof(interestId));

            _repo.Delete(userId, interestId);
        }

        public void ActualizarUserInterest(int userId, int oldInterestId, int newInterestId)
        {
            if (userId <= 0)
                throw new ArgumentException("El ID de usuario debe ser mayor que cero.", nameof(userId));
            if (oldInterestId <= 0)
                throw new ArgumentException("El ID de interés (antiguo) debe ser mayor que cero.", nameof(oldInterestId));
            if (newInterestId <= 0)
                throw new ArgumentException("El ID de interés (nuevo) debe ser mayor que cero.", nameof(newInterestId));

            _repo.Update(userId, oldInterestId, newInterestId);
        }

        public List<UserInterest> ObtenerTodos()
        {
            var entities = _repo.GetAll();
            var dtos = new List<UserInterest>(entities.Count);

            foreach (var e in entities)
            {
                dtos.Add(new UserInterest
                {
                    UserId = e.UserId,
                    InterestId = e.InterestId
                });
            }

            return dtos;
        }
    }
}
