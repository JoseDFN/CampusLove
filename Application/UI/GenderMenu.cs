using System;
using System.Collections.Generic;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Application.UI;

namespace CampusLove.Application.UI
{
    public class GenderMenu : BaseMenu
    {
        private readonly GenderService _service;

        public GenderMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpGenderRepository(connStr);
            _service = new GenderService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE GÉNEROS");
                Console.WriteLine("1. Crear Género");
                Console.WriteLine("2. Actualizar Género");
                Console.WriteLine("3. Eliminar Género");
                Console.WriteLine("4. Listar Géneros");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearGenero();
                        break;
                    case 2:
                        ActualizarGenero();
                        break;
                    case 3:
                        EliminarGenero();
                        break;
                    case 4:
                        ListarGeneros();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearGenero()
        {
            ShowHeader("CREAR NUEVO GÉNERO");
            string desc = GetValidatedInput("Descripción del género: ");

            var gender = new Gender { Description = desc };
            try
            {
                _service.CrearGenero(gender);
                ShowSuccessMessage("Género creado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear género: {ex.Message}");
            }
        }

        private void ActualizarGenero()
        {
            ShowHeader("ACTUALIZAR GÉNERO");
            int id = GetValidatedIntInput("ID del género a actualizar: ", 1);
            string desc = GetValidatedInput("Nueva descripción del género: ");

            var gender = new Gender { GenderId = id, Description = desc };
            try
            {
                _service.ActualizarGenero(gender);
                ShowSuccessMessage("Género actualizado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar género: {ex.Message}");
            }
        }

        private void EliminarGenero()
        {
            ShowHeader("ELIMINAR GÉNERO");
            int id = GetValidatedIntInput("ID del género a eliminar: ", 1);

            try
            {
                _service.EliminarGenero(id);
                ShowSuccessMessage("Género eliminado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar género: {ex.Message}");
            }
        }

        public void ListarGeneros()
        {
            ShowHeader("LISTADO DE GÉNEROS");
            try
            {
                _service.MostrarTodos();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al listar géneros: {ex.Message}");
            }
        }
    }
}
