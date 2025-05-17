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
    public class ImpInterestsRepository : IGenericRepository<Interest>, IInterestRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpInterestsRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Interest entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO interest (description)
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
                    $"No se pudo insertar interest (description = '{entity.Description}').");
        }

        public List<Interest> GetAll()
        {
            var list = new List<Interest>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT interest_id, description
                  FROM interest
                 ORDER BY interest_id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Interest
                {
                    InterestId  = rdr.GetInt32(0),
                    Description = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(Interest entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE interest
                   SET description = @description
                 WHERE interest_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id", entity.InterestId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró interest con id = {entity.InterestId} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM interest
                 WHERE interest_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró interest con id = {id} para eliminar.");
        }
    }
}
