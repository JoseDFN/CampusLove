using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class CareerService
    {
        private readonly IGenericRepository<Career> _repo;

        public CareerService(IGenericRepository<Career> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Crea una nueva carrera.
        /// </summary>
        public void CrearCareer(Career career)
        {
            if (career == null)
                throw new ArgumentNullException(nameof(career));
            if (string.IsNullOrWhiteSpace(career.Name))
                throw new ArgumentException("El nombre de la carrera no puede estar vacío.", nameof(career.Name));

            _repo.Create(career);
        }

        /// <summary>
        /// Elimina la carrera con el id especificado.
        /// </summary>
        public void EliminarCareer(int careerId)
        {
            if (careerId <= 0)
                throw new ArgumentException("El ID de carrera debe ser mayor que cero.", nameof(careerId));

            _repo.Delete(careerId);
        }

        /// <summary>
        /// Actualiza los datos de una carrera existente.
        /// </summary>
        public void ActualizarCareer(Career career)
        {
            if (career == null)
                throw new ArgumentNullException(nameof(career));
            if (career.CareerId <= 0)
                throw new ArgumentException("El ID de carrera debe ser mayor que cero.", nameof(career.CareerId));
            if (string.IsNullOrWhiteSpace(career.Name))
                throw new ArgumentException("El nombre de la carrera no puede estar vacío.", nameof(career.Name));

            _repo.Update(career);
        }

        /// <summary>
        /// Obtiene todas las carreras.
        /// </summary>
        public List<Career> ObtenerTodos()
        {
            return _repo.GetAll();
        }
    }
}