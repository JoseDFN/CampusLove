using System;
using System.Collections.Generic;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class SexualOrientationService
    {
        private readonly ISexualOrientationRepository _repo;

        public SexualOrientationService(ISexualOrientationRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public void CrearOrientacionSexual(SexualOrientation orientation)
        {
            if (orientation == null)
                throw new ArgumentNullException(nameof(orientation));
            if (string.IsNullOrWhiteSpace(orientation.Description))
                throw new ArgumentException("La descripción de la orientación sexual no puede estar vacía.", nameof(orientation.Description));

            _repo.Create(orientation);
        }

        public void EliminarOrientacion(int orientationId)
        {
            if (orientationId <= 0)
                throw new ArgumentException("El ID de la orientación sexual debe ser mayor que cero.", nameof(orientationId));

            _repo.Delete(orientationId);
        }

        public void ActualizarOrientacion(SexualOrientation orientation)
        {
            if (orientation == null)
                throw new ArgumentNullException(nameof(orientation));
            if (orientation.OrientationId <= 0)
                throw new ArgumentException("El ID de la orientación sexual debe ser mayor que cero.", nameof(orientation.OrientationId));
            if (string.IsNullOrWhiteSpace(orientation.Description))
                throw new ArgumentException("La descripción de la orientación sexual no puede estar vacía.", nameof(orientation.Description));

            _repo.Update(orientation);
        }

        public void MostrarTodos()
        {
            var lista = _repo.GetAll();
            if (lista == null || lista.Count == 0)
            {
                Console.WriteLine("No hay orientaciones sexuales registradas.");
                return;
            }

            Console.WriteLine("ID\tDescripción");
            foreach (var o in lista)
            {
                Console.WriteLine($"{o.OrientationId}\t{o.Description}");
            }
        }
    }
}
