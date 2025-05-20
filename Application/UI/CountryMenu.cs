using System;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Application.UI;

namespace CampusLove.Application.UI
{
    public class CountryMenu : BaseMenu
    {
        private readonly CountryService _service;

        public CountryMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpCountryRepository(connStr);
            _service = new CountryService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE PAÍSES");
                Console.WriteLine("1. Crear País");
                Console.WriteLine("2. Actualizar País");
                Console.WriteLine("3. Eliminar País");
                Console.WriteLine("4. Listar Países");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearPais();
                        break;
                    case 2:
                        ActualizarPais();
                        break;
                    case 3:
                        EliminarPais();
                        break;
                    case 4:
                        ListarPaises();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearPais()
        {
            ShowHeader("CREAR NUEVO PAÍS");
            string name = GetValidatedInput("Nombre del país: ");

            var country = new Country { Name = name };
            try
            {
                _service.CrearCountry(country);
                ShowSuccessMessage("País creado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear país: {ex.Message}");
            }
        }

        private void ActualizarPais()
        {
            ShowHeader("ACTUALIZAR PAÍS");
            int id = GetValidatedIntInput("ID del país a actualizar: ", 1);
            string name = GetValidatedInput("Nuevo nombre del país: ");

            var country = new Country { Id = id, Name = name };
            try
            {
                _service.ActualizarCountry(country);
                ShowSuccessMessage("País actualizado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar país: {ex.Message}");
            }
        }

        private void EliminarPais()
        {
            ShowHeader("ELIMINAR PAÍS");
            int id = GetValidatedIntInput("ID del país a eliminar: ", 1);

            try
            {
                _service.EliminarCountry(id);
                ShowSuccessMessage("País eliminado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar país: {ex.Message}");
            }
        }

        public void ListarPaises()
        {
            ShowHeader("LISTADO DE PAÍSES");
            try
            {
                _service.MostrarTodos();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al listar países: {ex.Message}");
            }
        }
    }
} 