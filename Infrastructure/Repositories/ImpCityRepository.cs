using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.infrastructure.postgres;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpCityRepository : IGenericRepository<City>, ICityRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpCityRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(City entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO city (name, region_id)
                VALUES (@name, @regionId);
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name",      entity.Name  ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("regionId",  entity.RegionId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se pudo insertar city (name = '{entity.Name}', region_id = {entity.RegionId}).");
        }

        public List<City> GetAll()
        {
            var list = new List<City>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT id, name, region_id
                  FROM city
                 ORDER BY id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new City
                {
                    Id        = rdr.GetInt32(0),
                    Name      = rdr.IsDBNull(1) ? null : rdr.GetString(1),
                    RegionId  = rdr.GetInt32(2)
                });
            }

            return list;
        }

        public void Update(City entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE city
                   SET name      = @name,
                       region_id = @regionId
                 WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name",      entity.Name  ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("regionId",  entity.RegionId);
            cmd.Parameters.AddWithValue("id",        entity.Id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró city con id = {entity.Id} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM city
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
                    $"No se encontró city con id = {id} para eliminar.");
        }
    }
}