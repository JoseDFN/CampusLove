using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using CampusLove.Application.Service;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;
using SGCI_app.application.UI;
using CampusLove.Application.UI;

public class LoginUI : BaseMenu
{
    private readonly AppUserService _service;

    public LoginUI(AppUserService service) : base(showIntro: false)
    {
        _service = service;
    }

    public void Login()
    {
        ShowHeader("INICIAR SESIÓN");

        string email = GetValidatedInput("Email: ");
        string password = GetValidatedPassword("Contraseña: ");

        try
        {
            Console.WriteLine("\nBuscando usuario...");
            // Buscar usuario por email
            DtoAppUser user = _service.ObtenerUsuarioPorEmail(email);

            if (user == null)
            {
                ShowErrorMessage("Usuario no encontrado.");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Usuario encontrado.");
            Console.WriteLine("Verificando contraseña...");

            // Verificar contraseña usando BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ShowErrorMessage("Contraseña incorrecta.");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            ShowSuccessMessage($"Bienvenido, {user.Name}");

            // Redirigir al menú según el tipo de usuario
            switch (user.UserTypeId)
            {
                case 1: // User
                    Console.WriteLine("Redirigiendo al menú de usuario...");
                    MostrarMenuUsuario(user);
                    break;

                case 2: // Admin
                    Console.WriteLine("Redirigiendo al menú de administrador...");
                    MostrarMenuAdmin(user);
                    break;

                default:
                    ShowErrorMessage($"Tipo de usuario desconocido: {user.UserTypeId}");
                    Console.ReadKey();
                    break;
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error en inicio de sesión: {ex.Message}");
            Console.WriteLine($"\nStackTrace: {ex.StackTrace}");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    private void MostrarMenuUsuario(DtoAppUser user)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"--- MENÚ USUARIO: {user.Name} ---");
            Console.WriteLine("1. Ver Perfiles (Feed)");
            Console.WriteLine("2. Ver mis coincidencias");
            Console.WriteLine("3. Ver estadísticas del sistema");
            Console.WriteLine("4. Actualizar perfil");
            Console.WriteLine("5. Salir");
            Console.Write("\nSeleccione una opción: ");
            
            string opcion = Console.ReadLine()!;
            
            switch (opcion)
            {
                case "1":
                    // Invocar FeedMenu
                    var feedMenu = new FeedMenu(user.UserId);
                    feedMenu.ShowMenu();
                    break;
                case "2":
                    Console.WriteLine("Función de ver coincidencias en desarrollo...");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
                case "3":
                    MostrarEstadisticas(user);
                    break;
                case "4":
                    ActualizarPerfilUsuario(user);
                    break;
                case "5":
                    Console.WriteLine("Cerrando sesión...");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void MostrarEstadisticas(DtoAppUser user)
    {
        try
        {
            Console.Clear();
            ShowHeader("ESTADÍSTICAS DEL SISTEMA");

            // Conexión y servicios
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
            var interactionRepo = new ImpInteractionRepository(connStr);
            var matchRepo = new ImpMatchRepository(connStr);
            var userRepo = new ImpAppUserRepository(connStr);
            var statsService = new StatisticsService(interactionRepo, matchRepo, userRepo, connStr);

            // Estadísticas
            var (totalLikes, totalMatches, matchRate) = statsService.GetSystemStatistics();
            var (userLikes, userMatches, userMatchRate) = statsService.GetUserStatistics(user.UserId);

            // Tabla comparativa
            Console.WriteLine("\n=== COMPARATIVA DE ESTADÍSTICAS ===");
            Console.WriteLine("{0,-35} {1,-15} {2,-15} {3}", "Métrica", "Tú", "Sistema", "Comparación");
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("{0,-35} {1,-15} {2,-15} {3}",
                "Likes enviados:",
                userLikes.ToString("N0"),
                totalLikes.ToString("N0"),
                totalLikes > 0 ? $"{(double)userLikes / totalLikes * 100:F2}% del total" : "N/A");
            Console.WriteLine("{0,-35} {1,-15} {2,-15} {3}",
                "Matches generados:",
                userMatches.ToString("N0"),
                totalMatches.ToString("N0"),
                totalMatches > 0 ? $"{(double)userMatches / totalMatches * 100:F2}% del total" : "N/A");
            Console.WriteLine("{0,-35} {1,-15:F2}% {2,-15:F2}% {3}",
                "Tasa de match (likes → match):",
                userMatchRate,
                matchRate,
                userMatchRate > matchRate ? "Superior al promedio" :
                userMatchRate < matchRate ? "Inferior al promedio" : "Igual al promedio");

            // Visualización gráfica
            Console.WriteLine("\nVisualización:");
            MostrarBarra("Tus likes", userLikes, totalLikes);
            MostrarBarra("Likes del sistema", totalLikes, totalLikes);

            MostrarBarra("Tu tasa de match", userMatchRate, 100);
            MostrarBarra("Tasa sistema", matchRate, 100);

            // Feedback
            Console.Write("\n");
            if (userMatchRate > matchRate)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("🔼 Estás consiguiendo más matches por like que el promedio del sistema. ¡Buen trabajo!");
            }
            else if (userMatchRate < matchRate)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("🔽 Tu tasa de match está por debajo del promedio. ¡Sigue intentándolo!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⏸️ Tu tasa de match está justo en el promedio del sistema.");
            }
            Console.ResetColor();

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error al mostrar estadísticas: {ex.Message}");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }

    // Método auxiliar para dibujar barras de progreso ASCII
    private void MostrarBarra(string etiqueta, double valor, double total, int ancho = 35)
    {
        if (total <= 0)
        {
            Console.WriteLine($"{etiqueta,-25} [No disponible]");
            return;
        }

        double porcentaje = (valor / total) * 100;
        int llenado = (int)(ancho * porcentaje / 100);
        string barra = new string('█', llenado) + new string('░', ancho - llenado);
        Console.WriteLine($"{etiqueta,-25} [{barra}] {porcentaje,6:F2}%");
    }


    private void ActualizarPerfilUsuario(DtoAppUser user)
    {
        while (true)
        {
            Console.Clear();
            ShowHeader("ACTUALIZAR PERFIL");
            Console.WriteLine("1. Datos Personales");
            Console.WriteLine("2. Carreras");
            Console.WriteLine("3. Intereses");
            Console.WriteLine("4. Salir");
            Console.Write("\nSeleccione una opción: ");

            string opcion = Console.ReadLine()!;

            switch (opcion)
            {
                case "1":
                    ActualizarDatosPersonales(user);
                    break;
                case "2":
                    ActualizarCarreras(user);
                    break;
                case "3":
                    ActualizarIntereses(user);
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void ActualizarDatosPersonales(DtoAppUser user)
    {
        try
        {
            ShowHeader("ACTUALIZAR DATOS PERSONALES");

            string name = GetValidatedInput("Nombre: ");
            
            // Validación de edad del usuario
            int age;
            do
            {
                age = GetValidatedIntInput("Edad: ", 0);
                if (age < 18)
                {
                    ShowErrorMessage("Lo sentimos, este programa no es apto para menores de edad.");
                    Console.Clear();
                    return;
                }
            } while (age < 18);

            // Leer contraseña ocultando entrada
            string password = GetValidatedPassword("Password: ");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            Console.Clear();
            var genderMenu = new GenderMenu();
            genderMenu.ListarGeneros();
            int genderId = GetValidatedIntInput("ID Género: ");

            string street = GetValidatedInput("Calle: ");
            string buildingNumber = GetValidatedInput("Número de edificio: ");
            string postalCode = GetValidatedInput("Código postal: ");
            Console.Clear();
            var cityMenu = new CityMenu();
            cityMenu.ListarCiudades();
            int cityId = GetValidatedIntInput("ID Ciudad: ");
            string additionalInfo = GetValidatedInput("Info adicional: ", allowEmpty: true);
            Console.Clear();
            var sexualOrientationMenu = new SexualOrientationMenu();
            sexualOrientationMenu.ListarOrientaciones();
            int orientationId = GetValidatedIntInput("ID Orientación: ");

            // Validación de edad mínima preferida
            int minAge;
            do
            {
                minAge = GetValidatedIntInput("Edad mínima preferida: ", 0);
                if (minAge < 18)
                {
                    ShowErrorMessage("La edad mínima debe ser 18 años o más, ya que el programa no es apto para menores de edad.");
                }
            } while (minAge < 18);

            // Validación de edad máxima preferida
            int maxAge;
            do
            {
                maxAge = GetValidatedIntInput("Edad máxima preferida: ", 0);
                if (maxAge < minAge)
                {
                    ShowErrorMessage($"La edad máxima debe ser mayor o igual a la edad mínima ({minAge} años).");
                }
            } while (maxAge < minAge);

            string profileText = GetValidatedInput("Texto de perfil: ", allowEmpty: true);

            var dto = new DtoAppUser
            {
                Name = name,
                Age = age,
                Email = user.Email, // Mantenemos el email original
                PasswordHash = passwordHash,
                GenderId = genderId,
                Address = new DtoAddr
                {
                    Street = street,
                    BuildingNumber = buildingNumber,
                    PostalCode = postalCode,
                    CityId = cityId,
                    AdditionalInfo = additionalInfo
                },
                UserProfile = new DtoUserProf
                {
                    Preference = new DtoPref
                    {
                        OrientationId = orientationId,
                        MinAge = minAge,
                        MaxAge = maxAge
                    },
                    ProfileText = profileText
                }
            };

            _service.ActualizarUsuario(user.UserId, dto);
            ShowSuccessMessage("Datos personales actualizados exitosamente.");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error al actualizar datos personales: {ex.Message}");
        }
    }

    private void ActualizarCarreras(DtoAppUser user)
    {
        try
        {
            Console.Clear();
            ShowHeader("ACTUALIZAR CARRERAS");
            Console.WriteLine("¿Desea actualizar sus carreras? (Y/N)");
            string respCarreras = GetValidatedInput("").ToUpper();
            
            if (respCarreras == "Y")
            {
                // Primero eliminamos todas las carreras actuales
                string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
                var userCareerService = new UserCareerService(new ImpUserCareerRepository(connStr));
                var carrerasActuales = userCareerService.ObtenerTodos().Where(uc => uc.UserId == user.UserId);
                foreach (var carrera in carrerasActuales)
                {
                    userCareerService.EliminarUserCareer(user.UserId, carrera.CareerId);
                }

                // Ahora agregamos las nuevas carreras
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
                        userCareerService.CrearUserCareer(new UserCareer { UserId = user.UserId, CareerId = careerId });
                        ShowSuccessMessage("Carrera agregada correctamente.");
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage($"Error al agregar carrera: {ex.Message}");
                    }

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
            }

            ShowSuccessMessage("Carreras actualizadas exitosamente.");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error al actualizar carreras: {ex.Message}");
        }
    }

    private void ActualizarIntereses(DtoAppUser user)
    {
        try
        {
            Console.Clear();
            ShowHeader("ACTUALIZAR INTERESES");
            Console.WriteLine("¿Desea actualizar sus intereses? (Y/N)");
            string respIntereses = GetValidatedInput("").ToUpper();
            
            if (respIntereses == "Y")
            {
                // Primero eliminamos todos los intereses actuales
                string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=1219;Pooling=true";
                var userInterestService = new UserInterestService(new ImpUserInterestsRepository(connStr));
                var interesesActuales = userInterestService.ObtenerTodos().Where(ui => ui.UserId == user.UserId);
                foreach (var interes in interesesActuales)
                {
                    userInterestService.EliminarUserInterest(user.UserId, interes.InterestId);
                }

                // Ahora agregamos los nuevos intereses
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
                        userInterestService.CrearUserInterest(new UserInterest { UserId = user.UserId, InterestId = interestId });
                        ShowSuccessMessage("Interés agregado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage($"Error al agregar interés: {ex.Message}");
                    }

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
            }

            ShowSuccessMessage("Intereses actualizados exitosamente.");
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error al actualizar intereses: {ex.Message}");
        }
    }

    private void MostrarMenuAdmin(DtoAppUser user)
    {
        try
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"--- MENÚ ADMINISTRADOR: {user.Name} ---");
                Console.WriteLine("1. Carreras");
                Console.WriteLine("2. Intereses");
                Console.WriteLine("3. Géneros");
                Console.WriteLine("4. Orientaciones Sexuales");
                Console.WriteLine("5. Países");
                Console.WriteLine("6. Regiones");
                Console.WriteLine("7. Ciudades");
                Console.WriteLine("8. Volver al menú anterior");
                Console.Write("\nSeleccione una opción: ");

                string opcion = Console.ReadLine()!;

                switch (opcion)
                {
                    case "1":
                        var careerMenu = new CareerMenu();
                        careerMenu.ShowMenu();
                        break;
                    case "2":
                        var interestMenu = new InterestMenu();
                        interestMenu.ShowMenu();
                        break;
                    case "3":
                        var genderMenu = new GenderMenu();
                        genderMenu.ShowMenu();
                        break;
                    case "4":
                        var orientationMenu = new SexualOrientationMenu();
                        orientationMenu.ShowMenu();
                        break;
                    case "5":
                        var countryMenu = new CountryMenu();
                        countryMenu.ShowMenu();
                        break;
                    case "6":
                        var regionMenu = new RegionMenu();
                        regionMenu.ShowMenu();
                        break;
                    case "7":
                        var cityMenu = new CityMenu();
                        cityMenu.ShowMenu();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en menú de administrador: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
    public override void ShowMenu()
    {
        Login();
    }
}
