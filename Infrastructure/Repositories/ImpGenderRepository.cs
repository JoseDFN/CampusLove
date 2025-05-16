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
    public class ImpGenderRepository : IGenericRepository<Gender>, IGenderRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpGenderRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Gender entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO gender (description)
                VALUES (@description);
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se pudo insertar gender (description = '{entity.Description}').");
        }

        public List<Gender> GetAll()
        {
            var list = new List<Gender>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT gender_id, description
                  FROM gender
                 ORDER BY gender_id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Gender
                {
                    GenderId    = rdr.GetInt32(0),
                    Description = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(Gender entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE gender
                   SET description = @description
                 WHERE gender_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id",          entity.GenderId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró gender con id = {entity.GenderId} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM gender
                 WHERE gender_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró gender con id = {id} para eliminar.");
        }
    }
}
