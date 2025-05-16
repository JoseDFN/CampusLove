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
    public class ImpCareerRepository : IGenericRepository<Career>, ICareerRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpCareerRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Career entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO career (name)
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
                    $"No se pudo insertar career (name = '{entity.Name}').");
        }

        public List<Career> GetAll()
        {
            var list = new List<Career>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT career_id, name
                  FROM career
                 ORDER BY career_id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Career
                {
                    CareerId = rdr.GetInt32(0),
                    Name     = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(Career entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE career
                   SET name = @name
                 WHERE career_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id",   entity.CareerId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró career con id = {entity.CareerId} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM career
                 WHERE career_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró career con id = {id} para eliminar.");
        }
    }
}
