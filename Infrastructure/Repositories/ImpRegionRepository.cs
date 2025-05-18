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
    public class ImpRegionRepository : IGenericRepository<Region>, IRegionRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpRegionRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Region entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO region (name, country_id)
                VALUES (@name, @countryId);
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("countryId", entity.CountryId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se pudo insertar region (name = '{entity.Name}', country_id = {entity.CountryId}).");
        }

        public List<Region> GetAll()
        {
            var list = new List<Region>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT id, name, country_id
                  FROM region
                 ORDER BY name;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Region
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? null : rdr.GetString(1),
                    CountryId = rdr.GetInt32(2)
                });
            }

            return list;
        }

        public void Update(Region entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE region
                   SET name = @name,
                       country_id = @countryId
                 WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("countryId", entity.CountryId);
            cmd.Parameters.AddWithValue("id", entity.Id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró region con id = {entity.Id} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM region
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
                    $"No se encontró region con id = {id} para eliminar.");
        }

        // Implementación de los métodos específicos de IRegionRepository
        public void CrearRegion(Region region) => Create(region);
        public void ActualizarRegion(Region region) => Update(region);
        public void EliminarRegion(int id) => Delete(id);
        public Region ObtenerRegion(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT id, name, country_id
                  FROM region
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
                return new Region
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? null : rdr.GetString(1),
                    CountryId = rdr.GetInt32(2)
                };
            }
            return null!;
        }
        public IEnumerable<Region> ObtenerTodos() => GetAll();
    }
} 