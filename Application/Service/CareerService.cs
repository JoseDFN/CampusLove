using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class CareerService
    {
        private readonly ICareerRepository _repo;

        public CareerService(ICareerRepository repo)
        {
            _repo = repo;
        }

        public void CrearCareer(Career career)
        {
            if (career == null)
                throw new ArgumentNullException(nameof(career));
            if (string.IsNullOrWhiteSpace(career.Name))
                throw new ArgumentException("El nombre de la carrera no puede estar vacío.", nameof(career.Name));

            _repo.Create(career);
        }

        public void EliminarCareer(int careerId)
        {
            if (careerId <= 0)
                throw new ArgumentException("El ID de carrera debe ser mayor que cero.", nameof(careerId));

            _repo.Delete(careerId);
        }

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

        public List<Career> ObtenerTodos()
        {
            return _repo.GetAll();
        }

        public void MostrarTodos()
        {
            var lista = _repo.GetAll();
            const int pageSize = 20;
            int totalPages = (int)Math.Ceiling(lista.Count / (double)pageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"LISTADO DE CARRERAS (Página {currentPage}/{totalPages})");
                Console.WriteLine(new string('-', 60));

                // Toma el bloque de 20 elementos de la página actual
                var pageItems = lista
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Muestra en dos columnas de 10 filas
                for (int i = 0; i < 10; i++)
                {
                    int leftIndex = i;
                    int rightIndex = i + 10;

                    string leftText = leftIndex < pageItems.Count
                        ? $"[{pageItems[leftIndex].CareerId,3}] {pageItems[leftIndex].Name,-20}"
                        : "";
                    string rightText = rightIndex < pageItems.Count
                        ? $"[{pageItems[rightIndex].CareerId,3}] {pageItems[rightIndex].Name,-20}"
                        : "";

                    Console.WriteLine($"{leftText}    {rightText}");
                }

                Console.WriteLine(new string('-', 60));
                Console.Write("N=Next, P=Prev, Q=Salir sub menu e ingresar Dato > ");
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.N && currentPage < totalPages)
                    currentPage++;
                else if (key == ConsoleKey.P && currentPage > 1)
                    currentPage--;
                else if (key == ConsoleKey.Q)
                    break;
            }
        }
    }
}