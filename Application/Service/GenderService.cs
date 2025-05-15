using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class GenderService
    {
        private readonly IGenericRepository<Gender> _repo;

        public GenderService(IGenericRepository<Gender> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        public void CrearCareer(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException(nameof(gender));
            if (string.IsNullOrWhiteSpace(gender.Description))
                throw new ArgumentException("El nombre de la carrera no puede estar vacío.", nameof(gender.Description));

            _repo.Create(gender);
        }
    }
}