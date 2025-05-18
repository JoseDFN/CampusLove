using System;
using System.Collections.Generic;
using System.Linq;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;

namespace CampusLove.Application.Service
{
    public class RegionService
    {
        private readonly IRegionRepository _repository;

        public RegionService(IRegionRepository repository)
        {
            _repository = repository;
        }

        public void CrearRegion(Region region)
        {
            _repository.CrearRegion(region);
        }

        public void ActualizarRegion(Region region)
        {
            _repository.ActualizarRegion(region);
        }

        public void EliminarRegion(int id)
        {
            _repository.EliminarRegion(id);
        }

        public Region ObtenerRegion(int id)
        {
            return _repository.ObtenerRegion(id);
        }

        public IEnumerable<Region> ObtenerTodos()
        {
            return _repository.ObtenerTodos();
        }

        public void MostrarTodos()
        {
            var regions = _repository.ObtenerTodos().ToList();
            int pageSize = 20;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)regions.Count / pageSize);

            while (true)
            {
                Console.Clear();
                ShowHeader("LISTADO DE REGIONES");
                Console.WriteLine($"Página {currentPage + 1} de {totalPages}");
                DrawSeparator();

                var pageRegions = regions.Skip(currentPage * pageSize).Take(pageSize).ToList();
                for (int i = 0; i < pageRegions.Count; i += 2)
                {
                    string left = $"[{pageRegions[i].Id}] {pageRegions[i].Name}";
                    string right = (i + 1 < pageRegions.Count) 
                        ? $"[{pageRegions[i + 1].Id}] {pageRegions[i + 1].Name}"
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