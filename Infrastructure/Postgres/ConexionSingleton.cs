using System;
using Npgsql;

namespace SGCI_app.infrastructure.postgres
{
    public class ConexionSingleton
    {
        private static ConexionSingleton? _instancia;
        private readonly string _connectionString;
        private NpgsqlConnection? _conexion;

        private ConexionSingleton(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static ConexionSingleton Instancia(string connectionString)
        {
            _instancia ??= new ConexionSingleton(connectionString);
            return _instancia;
        }

        public NpgsqlConnection ObtenerConexion()
        {
            _conexion ??= new NpgsqlConnection(_connectionString);

            if (_conexion.State != System.Data.ConnectionState.Open)
                _conexion.Open();

            return _conexion;
        }
    }
}
