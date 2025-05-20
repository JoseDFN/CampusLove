using System;
using CampusLove.Application.Service;
using CampusLove.Application.UI;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Entities;
using CampusLove.Infrastructure.Repositories;
using CampusLove.Infrastructure.Scheduling;

namespace CampusLove.ConsoleApp
{
    public class MainMenu : BaseMenu
    {
        private readonly AppUserService _userService;
        private readonly CareerService _careerService;
        private readonly UserCareerService _userCareerService;
        private readonly UserInterestService _userInterestService;
        private readonly InteractionCreditsService _creditsService;
        private readonly DailyInteractionCreditsScheduler _scheduler;
        private readonly CancellationTokenSource _cts;

        public MainMenu() : base(showIntro: true)
        {
            // Connection string - ajusta según tu entorno
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";

            _userService = new AppUserService(new ImpAppUserRepository(connStr));
            _careerService = new CareerService(new ImpCareerRepository(connStr));
            _userCareerService = new UserCareerService(new ImpUserCareerRepository(connStr));
            _careerService = new CareerService(new ImpCareerRepository(connStr));
            _userInterestService = new UserInterestService(new ImpUserInterestsRepository(connStr));

            // Repositorio, servicio de créditos y scheduler
            var creditsRepo = new ImpInteractionCreditsRepository(connStr);
            _creditsService = new InteractionCreditsService(creditsRepo);
            _scheduler = new DailyInteractionCreditsScheduler(_creditsService);
            _cts = new CancellationTokenSource();

            // **1. Al arrancar, asegúrate de que, si no se ha reseteado hoy, se ejecute el reset**
            _creditsService.EnsureDailyReset();

            // **2. Luego arranca el scheduler para futuras medianoches UTC**
            _scheduler.Start(_cts.Token);
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
                        _cts.Cancel();
                        return;
                }
            }
        }

        private void SignUpWithCareerFlow()
        {
            try
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
                    string resp;
                    bool validResponse;
                    do
                    {
                        resp = GetValidatedInput("¿Desea agregar otra carrera? (Y/N): ");
                        validResponse = resp.Equals("Y", StringComparison.OrdinalIgnoreCase) || resp.Equals("N", StringComparison.OrdinalIgnoreCase);
                        if (!validResponse)
                            ShowErrorMessage("Respuesta inválida. Por favor ingrese 'Y' o 'N'.");
                    } while (!validResponse);

                    addMore = resp.Equals("Y", StringComparison.OrdinalIgnoreCase);
                } while (addMore);

                // --- AGREGAR INTERESES ---
                bool addMoreInterests;
                do
                {
                    ShowHeader("AGREGAR INTERÉS");
                    var interestsMenu = new InterestMenu();
                    interestsMenu.ListarIntereses();
                    DrawSeparator();

                    int interestId = GetValidatedIntInput("Seleccione el ID del interés: ", 1);
                    try
                    {
                        _userInterestService.CrearUserInterest(new UserInterest { UserId = userId, InterestId = interestId });
                        ShowSuccessMessage("Interés agregado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage($"Error al agregar interés: {ex.Message}");
                    }

                    // Validar respuesta Y/N
                    string resp2;
                    bool valid2;
                    do
                    {
                        resp2 = GetValidatedInput("¿Desea agregar otro interés? (Y/N): ");
                        valid2 = resp2.Equals("Y", StringComparison.OrdinalIgnoreCase)
                              || resp2.Equals("N", StringComparison.OrdinalIgnoreCase);
                        if (!valid2)
                            ShowErrorMessage("Respuesta inválida. Ingrese 'Y' o 'N'.");
                    } while (!valid2);

                    addMoreInterests = resp2.Equals("Y", StringComparison.OrdinalIgnoreCase);

                } while (addMoreInterests);

                ShowSuccessMessage("Proceso de Sign Up completado.");
            }
            catch (InvalidOperationException ex) when (ex.Message == "MENOR_DE_EDAD")
            {
                // La excepción ya fue manejada en AppUserMenu, solo retornamos al menú principal
                return;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error en el proceso de registro: {ex.Message}");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}
