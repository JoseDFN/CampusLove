using System;
using System.Collections.Generic;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using SGCI_app.application.UI;

namespace CampusLove.Application.UI
{
    public class InterestMenu : BaseMenu
    {
        private readonly InterestService _service;

        public InterestMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpInterestsRepository(connStr);
            _service = new InterestService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE INTERESES");
                Console.WriteLine("1. Crear Interés");
                Console.WriteLine("2. Actualizar Interés");
                Console.WriteLine("3. Eliminar Interés");
                Console.WriteLine("4. Listar Intereses");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearInterest();
                        break;
                    case 2:
                        ActualizarInterest();
                        break;
                    case 3:
                        EliminarInterest();
                        break;
                    case 4:
                        ListarIntereses();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearInterest()
        {
            ShowHeader("CREAR NUEVO INTERÉS");
            string description = GetValidatedInput("Descripción del interés: ");

            var interest = new Interest { Description = description };
            try
            {
                _service.CrearInterest(interest);
                ShowSuccessMessage("Interés creado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear interés: {ex.Message}");
            }
        }

        private void ActualizarInterest()
        {
            ShowHeader("ACTUALIZAR INTERÉS");
            int id = GetValidatedIntInput("ID del interés a actualizar: ", 1);
            string description = GetValidatedInput("Nueva descripción del interés: ");

            var interest = new Interest { InterestId = id, Description = description };
            try
            {
                _service.ActualizarInterest(interest);
                ShowSuccessMessage("Interés actualizado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar interés: {ex.Message}");
            }
        }

        private void EliminarInterest()
        {
            ShowHeader("ELIMINAR INTERÉS");
            int id = GetValidatedIntInput("ID del interés a eliminar: ", 1);

            try
            {
                _service.EliminarInterest(id);
                ShowSuccessMessage("Interés eliminado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar interés: {ex.Message}");
            }
        }

        public void ListarIntereses()
        {
            ShowHeader("LISTADO DE INTERESES");
            try
            {
                var list = _service.ObtenerTodos();
                if (list.Count == 0)
                {
                    Console.WriteLine("No hay intereses registrados.");
                }
                else
                {
                    Console.WriteLine("ID\tDescripción");
                    foreach (var i in list)
                    {
                        Console.WriteLine($"{i.InterestId}\t{i.Description}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al obtener listado de intereses: {ex.Message}");
            }
        }
    }
}
