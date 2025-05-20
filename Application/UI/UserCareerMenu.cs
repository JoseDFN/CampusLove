using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;

namespace CampusLove.Application.UI
{
    public class UserCareerMenu : BaseMenu
    {
        private readonly UserCareerService _service;

        public UserCareerMenu() : base(showIntro: false)
        {
            // Ajusta tu connection string según corresponda
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            IUserCareerRepository repo = new ImpUserCareerRepository(connStr);
            _service = new UserCareerService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE VÍNCULOS USER–CAREER");
                Console.WriteLine("1. Crear vínculo");
                Console.WriteLine("2. Actualizar vínculo");
                Console.WriteLine("3. Eliminar vínculo");
                Console.WriteLine("4. Listar todos");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1: CrearVinculo();     break;
                    case 2: ActualizarVinculo(); break;
                    case 3: EliminarVinculo();   break;
                    case 4: ListarTodos();       break;
                    case 0: return;
                }
            }
        }

        private void CrearVinculo()
        {
            ShowHeader("CREAR VÍNCULO USER–CAREER");
            int userId   = GetValidatedIntInput("ID Usuario: ", 1);
            int careerId = GetValidatedIntInput("ID Carrera: ", 1);

            var dto = new UserCareer
            {
                UserId   = userId,
                CareerId = careerId
            };

            try
            {
                _service.CrearUserCareer(dto);
                ShowSuccessMessage("Vínculo creado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear vínculo: {ex.Message}");
            }
        }

        private void ActualizarVinculo()
        {
            ShowHeader("ACTUALIZAR VÍNCULO USER–CAREER");
            int userId      = GetValidatedIntInput("ID Usuario: ", 1);
            int oldCareerId = GetValidatedIntInput("ID Carrera actual: ", 1);
            int newCareerId = GetValidatedIntInput("ID Carrera nueva: ", 1);

            try
            {
                _service.ActualizarUserCareer(userId, oldCareerId, newCareerId);
                ShowSuccessMessage("Vínculo actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar vínculo: {ex.Message}");
            }
        }

        private void EliminarVinculo()
        {
            ShowHeader("ELIMINAR VÍNCULO USER–CAREER");
            int userId   = GetValidatedIntInput("ID Usuario: ", 1);
            int careerId = GetValidatedIntInput("ID Carrera: ", 1);

            try
            {
                _service.EliminarUserCareer(userId, careerId);
                ShowSuccessMessage("Vínculo eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar vínculo: {ex.Message}");
            }
        }

        private void ListarTodos()
        {
            ShowHeader("LISTA DE TODOS LOS VÍNCULOS");
            try
            {
                var list = _service.ObtenerTodos();
                if (list.Count == 0)
                {
                    Console.WriteLine("No hay vínculos registrados.");
                }
                else
                {
                    Console.WriteLine(" UserId | CareerId ");
                    Console.WriteLine("--------+----------");
                    foreach (var dto in list)
                    {
                        Console.WriteLine($"{dto.UserId,6} | {dto.CareerId,8}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al listar vínculos: {ex.Message}");
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}