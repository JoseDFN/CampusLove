using System;
using System.Collections.Generic;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using SGCI_app.application.UI;

namespace CampusLove.Application.UI
{
    public class UserTypeMenu : BaseMenu
    {
        private readonly UserTypeService _service;

        public UserTypeMenu() : base(showIntro: false)
        {
            // Ajusta la cadena de conexión según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var repo = new ImpUserTypeRepository(connStr);
            _service = new UserTypeService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE TIPOS DE USUARIO");
                Console.WriteLine("1. Crear Tipo de Usuario");
                Console.WriteLine("2. Actualizar Tipo de Usuario");
                Console.WriteLine("3. Eliminar Tipo de Usuario");
                Console.WriteLine("4. Listar Tipos de Usuario");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearTipoUsuario();
                        break;
                    case 2:
                        ActualizarTipoUsuario();
                        break;
                    case 3:
                        EliminarTipoUsuario();
                        break;
                    case 4:
                        ListarTiposUsuario();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void CrearTipoUsuario()
        {
            ShowHeader("CREAR TIPO DE USUARIO");
            string desc = GetValidatedInput("Descripción del tipo de usuario: ");

            var userType = new UserType { Description = desc };
            try
            {
                _service.CrearTipoUsuario(userType);
                ShowSuccessMessage("Tipo de usuario creado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear: {ex.Message}");
            }
        }

        private void ActualizarTipoUsuario()
        {
            ShowHeader("ACTUALIZAR TIPO DE USUARIO");
            int id = GetValidatedIntInput("ID del tipo de usuario a actualizar: ", 1);
            string desc = GetValidatedInput("Nueva descripción: ");

            var userType = new UserType { Id = id, Description = desc };
            try
            {
                _service.ActualizarGenero(userType);
                ShowSuccessMessage("Tipo de usuario actualizado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar: {ex.Message}");
            }
        }

        private void EliminarTipoUsuario()
        {
            ShowHeader("ELIMINAR TIPO DE USUARIO");
            int id = GetValidatedIntInput("ID del tipo de usuario a eliminar: ", 1);

            try
            {
                _service.EliminarGenero(id);
                ShowSuccessMessage("Tipo de usuario eliminado correctamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar: {ex.Message}");
            }
        }

        public void ListarTiposUsuario()
        {
            ShowHeader("LISTADO DE TIPOS DE USUARIO");
            try
            {
                var list = _service.ObtenerTodos();
                if (list.Count == 0)
                {
                    Console.WriteLine("No hay tipos de usuario registrados.");
                }
                else
                {
                    Console.WriteLine("ID\tDescripción");
                    foreach (var ut in list)
                    {
                        Console.WriteLine($"{ut.Id}\t{ut.Description}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al obtener el listado: {ex.Message}");
            }
        }
    }
}
