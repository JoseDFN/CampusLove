using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.infrastructure.postgres;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpCountryRepository : IGenericRepository<Country>, ICountryRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpCountryRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Country entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO country (name)
                VALUES (@name);
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se pudo insertar country (name = '{entity.Name}').");
        }

        public List<Country> GetAll()
        {
            var list = new List<Country>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT id, name
                  FROM country
                 ORDER BY name;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Country
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(Country entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE country
                   SET name = @name
                 WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id", entity.Id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró country con id = {entity.Id} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM country
                 WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró country con id = {id} para eliminar.");
        }

        // Implementación de los métodos específicos de ICountryRepository
        public void CrearCountry(Country country) => Create(country);
        public void ActualizarCountry(Country country) => Update(country);
        public void EliminarCountry(int id) => Delete(id);
        public Country ObtenerCountry(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT id, name
                  FROM country
                 WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("id", id);

            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new Country
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                };
            }
            return null!;
        }
        public IEnumerable<Country> ObtenerTodos() => GetAll();
    }
} 