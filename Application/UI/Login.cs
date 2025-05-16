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

public class LoginUI
{
    private readonly AppUserService _service;

    public LoginUI(AppUserService service)
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
            Console.WriteLine("\nPresione cualquier tecla para continuar al menú...");
            Console.ReadKey();

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
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
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
            Console.WriteLine("1. Ver perfil");
            Console.WriteLine("2. Buscar usuarios");
            Console.WriteLine("3. Cerrar sesión");
            Console.Write("\nSeleccione una opción: ");
            
            string opcion = Console.ReadLine()!;
            
            switch (opcion)
            {
                case "1":
                    // TODO: Implementar ver perfil
                    Console.WriteLine("Función de ver perfil en desarrollo...");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
                case "2":
                    // TODO: Implementar búsqueda de usuarios
                    Console.WriteLine("Función de búsqueda de usuarios en desarrollo...");
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
                case "3":
                    Console.WriteLine("Cerrando sesión...");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Presione cualquier tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
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
                Console.WriteLine("1. Gestión de usuarios");
                Console.WriteLine("2. Reportes");
                Console.WriteLine("3. Cerrar sesión");
                Console.Write("\nSeleccione una opción: ");
                
                string opcion = Console.ReadLine()!;
                Console.WriteLine($"Opción seleccionada: {opcion}");
                
                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("Abriendo menú de gestión de usuarios...");
                        var userMenu = new AppUserMenu();
                        userMenu.ShowMenu();
                        break;
                    case "2":
                        Console.WriteLine("Función de reportes en desarrollo...");
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.WriteLine("Cerrando sesión...");
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
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

    // Métodos auxiliares para entradas
    private string GetValidatedInput(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()!;
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write("Entrada no válida. " + prompt);
            input = Console.ReadLine()!;
        }
        return input;
    }

    private string GetValidatedPassword(string prompt)
    {
        Console.Write(prompt);
        string password = string.Empty;
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    private void ShowHeader(string title)
    {
        Console.Clear();
        Console.WriteLine("===================================");
        Console.WriteLine($"        {title}");
        Console.WriteLine("===================================");
    }

    private void ShowSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void ShowErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
