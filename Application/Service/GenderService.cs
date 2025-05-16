using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class GenderService
    {
        private readonly IGenderRepository _repo;

        public GenderService(IGenderRepository repo)
        {
            _repo = repo;
        }
        public void CrearGenero(Gender gender)
        {
            if (gender == null)
                throw new ArgumentNullException(nameof(gender));
            if (string.IsNullOrWhiteSpace(gender.Description))
                throw new ArgumentException("El nombre del genero no puede estar vacío.", nameof(gender.Description));

            _repo.Create(gender);
        }
        public void EliminarGenero(int generoId)
        {
            if (generoId <= 0)
                throw new ArgumentException("El ID del genero debe ser mayor que cero.", nameof(generoId));

            _repo.Delete(generoId);
        }
        public void ActualizarGenero(Gender genero)
        {
            if (genero == null)
                throw new ArgumentNullException(nameof(genero));
            if (genero.GenderId <= 0)
                throw new ArgumentException("El ID del genero debe ser mayor que cero.", nameof(genero.GenderId));
            if (string.IsNullOrWhiteSpace(genero.Description))
                throw new ArgumentException("El nombre del genero no puede estar vacío.", nameof(genero.Description));

            _repo.Update(genero);
        }

        public void MostrarTodos()
        {
            var lista = _repo.GetAll();
            if (lista.Count == 0)
            {
                Console.WriteLine("No hay géneros registrados.");
                return;
            }

            Console.WriteLine("ID\tDescripción");
            foreach (var g in lista)
            {
                Console.WriteLine($"{g.GenderId}\t{g.Description}");
            }
        }
    }
}