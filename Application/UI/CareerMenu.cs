using System;
using System.Collections.Generic;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using SGCI_app.application.UI;

namespace CampusLove.Application.UI
{
    public class CareerMenu : BaseMenu
    {
        private readonly CareerService _service;

        public CareerMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpCareerRepository(connStr);
            _service = new CareerService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE CARRERAS");
                Console.WriteLine("1. Crear Carrera");
                Console.WriteLine("2. Actualizar Carrera");
                Console.WriteLine("3. Eliminar Carrera");
                Console.WriteLine("4. Listar Carreras");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearCareer();
                        break;
                    case 2:
                        ActualizarCareer();
                        break;
                    case 3:
                        EliminarCareer();
                        break;
                    case 4:
                        ListarCarreras();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearCareer()
        {
            ShowHeader("CREAR NUEVA CARRERA");
            string name = GetValidatedInput("Nombre de la carrera: ");

            var career = new Career { Name = name };
            try
            {
                _service.CrearCareer(career);
                ShowSuccessMessage("Carrera creada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear carrera: {ex.Message}");
            }
        }

        private void ActualizarCareer()
        {
            ShowHeader("ACTUALIZAR CARRERA");
            int id = GetValidatedIntInput("ID de la carrera a actualizar: ", 1);
            string name = GetValidatedInput("Nuevo nombre de la carrera: ");

            var career = new Career { CareerId = id, Name = name };
            try
            {
                _service.ActualizarCareer(career);
                ShowSuccessMessage("Carrera actualizada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar carrera: {ex.Message}");
            }
        }

        private void EliminarCareer()
        {
            ShowHeader("ELIMINAR CARRERA");
            int id = GetValidatedIntInput("ID de la carrera a eliminar: ", 1);

            try
            {
                _service.EliminarCareer(id);
                ShowSuccessMessage("Carrera eliminada correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar carrera: {ex.Message}");
            }
        }

        public void ListarCarreras()
        {
            ShowHeader("LISTADO DE CARRERAS");
            try
            {
                _service.MostrarTodos();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al obtener listado de carreras: {ex.Message}");
            }
        }
    }
}
