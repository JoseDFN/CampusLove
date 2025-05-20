using System;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Application.UI;

namespace CampusLove.Application.UI
{
    public class CityMenu : BaseMenu
    {
        private readonly CityService _service;

        public CityMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpCityRepository(connStr);
            _service = new CityService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE CIUDADES");
                Console.WriteLine("1. Crear Ciudad");
                Console.WriteLine("2. Actualizar Ciudad");
                Console.WriteLine("3. Eliminar Ciudad");
                Console.WriteLine("4. Listar Ciudades");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearCiudad();
                        break;
                    case 2:
                        ActualizarCiudad();
                        break;
                    case 3:
                        EliminarCiudad();
                        break;
                    case 4:
                        ListarCiudades();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearCiudad()
        {
            ShowHeader("CREAR NUEVA CIUDAD");
            string name = GetValidatedInput("Nombre de la ciudad: ");
            int regionId = GetValidatedIntInput("ID de región: ", 1);

            var city = new City { Name = name, RegionId = regionId };
            try
            {
                _service.CrearCity(city);
                ShowSuccessMessage("Ciudad creada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear ciudad: {ex.Message}");
            }
        }

        private void ActualizarCiudad()
        {
            ShowHeader("ACTUALIZAR CIUDAD");
            int id = GetValidatedIntInput("ID de la ciudad a actualizar: ", 1);
            string name = GetValidatedInput("Nuevo nombre de la ciudad: ");
            int regionId = GetValidatedIntInput("Nuevo ID de región: ", 1);

            var city = new City { Id = id, Name = name, RegionId = regionId };
            try
            {
                _service.ActualizarCity(city);
                ShowSuccessMessage("Ciudad actualizada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar ciudad: {ex.Message}");
            }
        }

        private void EliminarCiudad()
        {
            ShowHeader("ELIMINAR CIUDAD");
            int id = GetValidatedIntInput("ID de la ciudad a eliminar: ", 1);

            try
            {
                _service.EliminarCity(id);
                ShowSuccessMessage("Ciudad eliminada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar ciudad: {ex.Message}");
            }
        }

        public void ListarCiudades()
        {
            ShowHeader("LISTADO DE CIUDADES");
            try
            {
                _service.MostrarTodos();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al listar ciudades: {ex.Message}");
            }
        }
    }
}
