using System;
using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class CountryService
    {
        private readonly ICountryRepository _repository;

        public CountryService(ICountryRepository repository)
        {
            _repository = repository;
        }

        public void CrearCountry(Country country)
        {
            _repository.CrearCountry(country);
        }

        public void ActualizarCountry(Country country)
        {
            _repository.ActualizarCountry(country);
        }

        public void EliminarCountry(int id)
        {
            _repository.EliminarCountry(id);
        }

        public Country ObtenerCountry(int id)
        {
            return _repository.ObtenerCountry(id);
        }

        public IEnumerable<Country> ObtenerTodos()
        {
            return _repository.ObtenerTodos();
        }

        public void MostrarTodos()
        {
            var countries = _repository.ObtenerTodos().ToList();
            int pageSize = 20;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)countries.Count / pageSize);

            while (true)
            {
                Console.Clear();
                ShowHeader("LISTADO DE PAÍSES");
                Console.WriteLine($"Página {currentPage + 1} de {totalPages}");
                DrawSeparator();

                var pageCountries = countries.Skip(currentPage * pageSize).Take(pageSize).ToList();
                for (int i = 0; i < pageCountries.Count; i += 2)
                {
                    string left = $"[{pageCountries[i].Id}] {pageCountries[i].Name}";
                    string right = (i + 1 < pageCountries.Count) 
                        ? $"[{pageCountries[i + 1].Id}] {pageCountries[i + 1].Name}"
                        : "";

                    Console.WriteLine($"{left,-40} {right}");
                }

                DrawSeparator();
                Console.WriteLine("N - Siguiente página");
                Console.WriteLine("P - Página anterior");
                Console.WriteLine("Q - Salir");
                Console.Write("\nSeleccione una opción: ");

                var key = Console.ReadKey(true).KeyChar.ToString().ToUpper();
                switch (key)
                {
                    case "N":
                        if (currentPage < totalPages - 1)
                            currentPage++;
                        break;
                    case "P":
                        if (currentPage > 0)
                            currentPage--;
                        break;
                    case "Q":
                        return;
                }
            }
        }

        private void ShowHeader(string title)
        {
            Console.WriteLine($"=== {title} ===");
        }

        private void DrawSeparator()
        {
            Console.WriteLine(new string('-', 80));
        }
    }
} 