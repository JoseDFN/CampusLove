using System;
using System.Collections.Generic;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class InterestService
    {
        private readonly IInterestRepository _repo;

        public InterestService(IInterestRepository repo)
        {
            _repo = repo;
        }

        public void CrearInterest(Interest interest)
        {
            if (interest == null)
                throw new ArgumentNullException(nameof(interest));
            if (string.IsNullOrWhiteSpace(interest.Description))
                throw new ArgumentException("La descripción del interés no puede estar vacía.", nameof(interest.Description));

            _repo.Create(interest);
        }

        public void EliminarInterest(int interestId)
        {
            if (interestId <= 0)
                throw new ArgumentException("El ID de interés debe ser mayor que cero.", nameof(interestId));

            _repo.Delete(interestId);
        }

        public void ActualizarInterest(Interest interest)
        {
            if (interest == null)
                throw new ArgumentNullException(nameof(interest));
            if (interest.InterestId <= 0)
                throw new ArgumentException("El ID de interés debe ser mayor que cero.", nameof(interest.InterestId));
            if (string.IsNullOrWhiteSpace(interest.Description))
                throw new ArgumentException("La descripción del interés no puede estar vacía.", nameof(interest.Description));

            _repo.Update(interest);
        }

        public List<Interest> ObtenerTodos()
        {
            return _repo.GetAll();
        }
    }
}
