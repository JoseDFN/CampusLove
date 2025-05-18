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

        public void MostrarTodos()
        {
            var lista = _repo.GetAll();
            const int pageSize = 20;
            int totalPages = (int)Math.Ceiling(lista.Count / (double)pageSize);
            int currentPage = 1;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"LISTADO DE INTERESES (Página {currentPage}/{totalPages})");
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
                        ? $"[{pageItems[leftIndex].InterestId,3}] {pageItems[leftIndex].Description,-20}"
                        : "";
                    string rightText = rightIndex < pageItems.Count
                        ? $"[{pageItems[rightIndex].InterestId,3}] {pageItems[rightIndex].Description,-20}"
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
