using System;
using System.Collections.Generic;
using CampusLove.Application.Service;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;

namespace CampusLove.Application.UI
{
    public class UserInterestMenu : BaseMenu
    {
        private readonly UserInterestService _service;

        public UserInterestMenu() : base(showIntro: false)
        {
            // Ajusta tu cadena de conexión según corresponda
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            IUserInterestRepository repo = new ImpUserInterestsRepository(connStr);
            _service = new UserInterestService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE VÍNCULOS USER–INTEREST");
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
            ShowHeader("CREAR VÍNCULO USER–INTEREST");
            int userId     = GetValidatedIntInput("ID Usuario: ", 1);
            int interestId = GetValidatedIntInput("ID Interés: ", 1);

            var dto = new UserInterest
            {
                UserId     = userId,
                InterestId = interestId
            };

            try
            {
                _service.CrearUserInterest(dto);
                ShowSuccessMessage("Vínculo creado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear vínculo: {ex.Message}");
            }
        }

        private void ActualizarVinculo()
        {
            ShowHeader("ACTUALIZAR VÍNCULO USER–INTEREST");
            int userId         = GetValidatedIntInput("ID Usuario: ", 1);
            int oldInterestId  = GetValidatedIntInput("ID Interés actual: ", 1);
            int newInterestId  = GetValidatedIntInput("ID Interés nuevo: ", 1);

            try
            {
                _service.ActualizarUserInterest(userId, oldInterestId, newInterestId);
                ShowSuccessMessage("Vínculo actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar vínculo: {ex.Message}");
            }
        }

        private void EliminarVinculo()
        {
            ShowHeader("ELIMINAR VÍNCULO USER–INTEREST");
            int userId     = GetValidatedIntInput("ID Usuario: ", 1);
            int interestId = GetValidatedIntInput("ID Interés: ", 1);

            try
            {
                _service.EliminarUserInterest(userId, interestId);
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
                    Console.WriteLine(" UserId | InterestId ");
                    Console.WriteLine("--------+------------");
                    foreach (var dto in list)
                    {
                        Console.WriteLine($"{dto.UserId,6} | {dto.InterestId,10}");
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
