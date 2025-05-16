using System;
using CampusLove.Application.Service;
using CampusLove.Application.UI;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using SGCI_app.application.UI;

namespace CampusLove.ConsoleApp
{
    public class MainMenu : BaseMenu
    {
        private readonly AppUserService _userService;
        private readonly CareerService _careerService;
        private readonly UserCareerService _userCareerService;

        public MainMenu() : base(showIntro: true)
        {
            // Connection string - ajusta según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=123456;Pooling=true";

            _userService = new AppUserService(new ImpAppUserRepository(connStr));
            _careerService = new CareerService(new ImpCareerRepository(connStr));
            _userCareerService = new UserCareerService(new ImpUserCareerRepository(connStr));
            _careerService = new CareerService(new ImpCareerRepository(connStr));
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("MENÚ PRINCIPAL");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Sign Up");
                Console.WriteLine("0. Salir");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 2);
                switch (option)
                {
                    case 1:
                        var loginUI = new LoginUI(_userService);
                        loginUI.Login();
                        break;
                    case 2:
                        SignUpWithCareerFlow();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private void SignUpWithCareerFlow()
        {
            // Crear usuario y obtener su ID
            var userMenu = new AppUserMenu();
            int userId = userMenu.CrearUsuario();

            bool addMore;
            do
            {
                ShowHeader("AGREGAR CARRERA");
                var careers = new CareerMenu();
                careers.ListarCarreras();
                DrawSeparator();

                int careerId = GetValidatedIntInput("Seleccione el ID de la carrera: ", 1);
                try
                {
                    _userCareerService.CrearUserCareer(new UserCareer { UserId = userId, CareerId = careerId });
                    ShowSuccessMessage("Carrera agregada correctamente.");
                }
                catch (Exception ex)
                {
                    ShowErrorMessage($"Error al agregar carrera: {ex.Message}");
                }

                // Preguntar si desea agregar otra
                string resp = GetValidatedInput("¿Desea agregar otra carrera? (Y/N): ");
                addMore = resp.Equals("Y", StringComparison.OrdinalIgnoreCase);

            } while (addMore);

            ShowSuccessMessage("Proceso de Sign Up completado.");
        }
    }
}
