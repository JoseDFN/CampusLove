using System;
using System.Collections.Generic;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;

namespace CampusLove.Application.UI
{
    public class SexualOrientationMenu : BaseMenu
    {
        private readonly SexualOrientationService _service;

        public SexualOrientationMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpSexualOrientationRepository(connStr);
            _service = new SexualOrientationService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE ORIENTACIONES SEXUALES");
                Console.WriteLine("1. Crear Orientación Sexual");
                Console.WriteLine("2. Actualizar Orientación Sexual");
                Console.WriteLine("3. Eliminar Orientación Sexual");
                Console.WriteLine("4. Listar Orientaciones Sexuales");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearOrientacion();
                        break;
                    case 2:
                        ActualizarOrientacion();
                        break;
                    case 3:
                        EliminarOrientacion();
                        break;
                    case 4:
                        ListarOrientaciones();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearOrientacion()
        {
            ShowHeader("CREAR NUEVA ORIENTACIÓN SEXUAL");
            string desc = GetValidatedInput("Descripción de la orientación sexual: ");

            var orientation = new SexualOrientation { Description = desc };
            try
            {
                _service.CrearOrientacionSexual(orientation);
                ShowSuccessMessage("Orientación sexual creada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear orientación sexual: {ex.Message}");
            }
        }

        private void ActualizarOrientacion()
        {
            ShowHeader("ACTUALIZAR ORIENTACIÓN SEXUAL");
            int id = GetValidatedIntInput("ID de la orientación sexual a actualizar: ", 1);
            string desc = GetValidatedInput("Nueva descripción de la orientación sexual: ");

            var orientation = new SexualOrientation { OrientationId = id, Description = desc };
            try
            {
                _service.ActualizarOrientacion(orientation);
                ShowSuccessMessage("Orientación sexual actualizada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar orientación sexual: {ex.Message}");
            }
        }

        private void EliminarOrientacion()
        {
            ShowHeader("ELIMINAR ORIENTACIÓN SEXUAL");
            int id = GetValidatedIntInput("ID de la orientación sexual a eliminar: ", 1);

            try
            {
                _service.EliminarOrientacion(id);
                ShowSuccessMessage("Orientación sexual eliminada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar orientación sexual: {ex.Message}");
            }
        }

        public void ListarOrientaciones()
        {
            ShowHeader("LISTADO DE ORIENTACIONES SEXUALES");
            try
            {
                _service.MostrarTodos();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al listar orientaciones sexuales: {ex.Message}");
            }
        }
    }
}
