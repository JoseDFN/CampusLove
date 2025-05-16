using System;
using System.Collections.Generic;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;

namespace CampusLove.Application.Service
{
    public class CityService
    {
        private readonly ICityRepository _repo;

        public CityService(ICityRepository repo)
        {
            _repo = repo;
        }

        public void CrearCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));
            if (string.IsNullOrWhiteSpace(city.Name))
                throw new ArgumentException("El nombre de la ciudad no puede estar vacío.", nameof(city.Name));
            if (city.RegionId <= 0)
                throw new ArgumentException("El ID de región debe ser mayor que cero.", nameof(city.RegionId));

            _repo.Create(city);
        }

        public void EliminarCity(int cityId)
        {
            if (cityId <= 0)
                throw new ArgumentException("El ID de ciudad debe ser mayor que cero.", nameof(cityId));

            _repo.Delete(cityId);
        }

        public void ActualizarCity(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));
            if (city.Id <= 0)
                throw new ArgumentException("El ID de ciudad debe ser mayor que cero.", nameof(city.Id));
            if (string.IsNullOrWhiteSpace(city.Name))
                throw new ArgumentException("El nombre de la ciudad no puede estar vacío.", nameof(city.Name));
            if (city.RegionId <= 0)
                throw new ArgumentException("El ID de región debe ser mayor que cero.", nameof(city.RegionId));

            _repo.Update(city);
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
                Console.WriteLine($"LISTADO DE CIUDADES (Página {currentPage}/{totalPages})");
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
                        ? $"[{pageItems[leftIndex].Id,3}] {pageItems[leftIndex].Name,-20}"
                        : "";
                    string rightText = rightIndex < pageItems.Count
                        ? $"[{pageItems[rightIndex].Id,3}] {pageItems[rightIndex].Name,-20}"
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
