using System;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;

namespace CampusLove.Application.UI
{
    public class RegionMenu : BaseMenu
    {
        private readonly RegionService _service;

        public RegionMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpRegionRepository(connStr);
            _service = new RegionService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE REGIONES");
                Console.WriteLine("1. Crear Región");
                Console.WriteLine("2. Actualizar Región");
                Console.WriteLine("3. Eliminar Región");
                Console.WriteLine("4. Listar Regiones");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearRegion();
                        break;
                    case 2:
                        ActualizarRegion();
                        break;
                    case 3:
                        EliminarRegion();
                        break;
                    case 4:
                        ListarRegiones();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearRegion()
        {
            ShowHeader("CREAR NUEVA REGIÓN");
            string name = GetValidatedInput("Nombre de la región: ");
            int countryId = GetValidatedIntInput("ID del país: ", 1);

            var region = new Region { Name = name, CountryId = countryId };
            try
            {
                _service.CrearRegion(region);
                ShowSuccessMessage("Región creada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear región: {ex.Message}");
            }
        }

        private void ActualizarRegion()
        {
            ShowHeader("ACTUALIZAR REGIÓN");
            int id = GetValidatedIntInput("ID de la región a actualizar: ", 1);
            string name = GetValidatedInput("Nuevo nombre de la región: ");
            int countryId = GetValidatedIntInput("Nuevo ID del país: ", 1);

            var region = new Region { Id = id, Name = name, CountryId = countryId };
            try
            {
                _service.ActualizarRegion(region);
                ShowSuccessMessage("Región actualizada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar región: {ex.Message}");
            }
        }

        private void EliminarRegion()
        {
            ShowHeader("ELIMINAR REGIÓN");
            int id = GetValidatedIntInput("ID de la región a eliminar: ", 1);

            try
            {
                _service.EliminarRegion(id);
                ShowSuccessMessage("Región eliminada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar región: {ex.Message}");
            }
        }

        public void ListarRegiones()
        {
            ShowHeader("LISTADO DE REGIONES");
            try
            {
                _service.MostrarTodos();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al listar regiones: {ex.Message}");
            }
        }
    }
} 