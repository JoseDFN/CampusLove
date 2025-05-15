using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using CampusLove.Application.Service;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;
using SGCI_app.application.UI;

namespace CampusLove.Application.UI
{
    public class AppUserMenu : BaseMenu
    {
        private readonly AppUserService _service;

        public AppUserMenu() : base(showIntro: false)
        {
            // Cadena de conexión, ajustar según sea necesario
            string connStr = "Host=localhost;Database=campus_love;Port=5432;Username=postgres;Password=campus2023;Pooling=true";
            IAppUserRepository repo = new ImpAppUserRepository(connStr);
            _service = new AppUserService(repo);
        }

        public override void ShowMenu()
        {
            while (true)
            {
                ShowHeader("GESTIÓN DE USUARIOS");
                Console.WriteLine("1. Crear Usuario");
                Console.WriteLine("2. Actualizar Usuario");
                Console.WriteLine("3. Eliminar Usuario");
                Console.WriteLine("0. Volver al menú principal");
                DrawSeparator();

                int option = GetValidatedIntInput("Seleccione una opción: ", 0, 4);
                switch (option)
                {
                    case 1:
                        CrearUsuario();
                        break;
                    case 2:
                        ActualizarUsuario();
                        break;
                    case 3:
                        EliminarUsuario();
                        break;
                    case 0:
                        return;
                }
            }
        }

        public int CrearUsuario()
        {
            ShowHeader("CREAR NUEVO USUARIO");

            string name = GetValidatedInput("Nombre: ");
            int age = GetValidatedIntInput("Edad: ", 0);
            string email = GetValidatedInput("Email: ");

            // Leer contraseña ocultando entrada
            string password = GetValidatedPassword("Password: ");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            int genderId = GetValidatedIntInput("ID Género: ");
            int userTypeId = GetValidatedIntInput("ID Tipo de Usuario: ");

            string street = GetValidatedInput("Calle: ");
            string buildingNumber = GetValidatedInput("Número de edificio: ");
            string postalCode = GetValidatedInput("Código postal: ");
            int cityId = GetValidatedIntInput("ID Ciudad: ");
            string additionalInfo = GetValidatedInput("Info adicional: ", allowEmpty: true);

            int orientationId = GetValidatedIntInput("ID Orientación: ");
            int minAge = GetValidatedIntInput("Edad mínima preferida: ");
            int maxAge = GetValidatedIntInput("Edad máxima preferida: ");

            string profileText = GetValidatedInput("Texto de perfil: ", allowEmpty: true);

            var dto = new DtoAppUser
            {
                Name = name,
                Age = age,
                Email = email,
                PasswordHash = passwordHash,
                GenderId = genderId,
                UserTypeId = userTypeId,
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

            try
            {
                int idUsuario = _service.CrearUsuario(dto);
                ShowSuccessMessage("Usuario creado exitosamente.");
                return idUsuario;
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al crear usuario: {ex.Message}");
                return -1;
            }
        }

        private void ActualizarUsuario()
        {
            ShowHeader("ACTUALIZAR USUARIO");
            int userId = GetValidatedIntInput("ID de usuario a actualizar: ", 1);

            // Solo campos actualizables
            string name = GetValidatedInput("Nuevo nombre: ", allowEmpty: true);
            int age = GetValidatedIntInput("Nueva edad: ");
            string email = GetValidatedInput("Nuevo email: ", allowEmpty: true);
            string password = GetValidatedPassword("Password: ");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            int genderId = GetValidatedIntInput("Nuevo ID Género: ");

            var dto = new DtoAppUser
            {
                Name = string.IsNullOrEmpty(name) ? null : name,
                Age = age,
                Email = string.IsNullOrEmpty(email) ? null : email,
                PasswordHash = string.IsNullOrEmpty(passwordHash) ? null : passwordHash,
                GenderId = genderId
            };

            try
            {
                _service.ActualizarUsuario(userId, dto);
                ShowSuccessMessage("Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al actualizar usuario: {ex.Message}");
            }
        }

        private void EliminarUsuario()
        {
            ShowHeader("ELIMINAR USUARIO");
            int userId = GetValidatedIntInput("ID de usuario a eliminar: ", 1);

            try
            {
                _service.EliminarUsuario(userId);
                ShowSuccessMessage("Usuario eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error al eliminar usuario: {ex.Message}");
            }
        }
    }
}