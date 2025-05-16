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
    public class ImpSexualOrientationRepository : IGenericRepository<SexualOrientation>, ISexualOrientationRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpSexualOrientationRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(SexualOrientation entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO sexual_orientation (description)
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
                    $"No se pudo insertar sexual_orientation (description = '{entity.Description}').");
        }

        public List<SexualOrientation> GetAll()
        {
            var list = new List<SexualOrientation>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT orientation_id, description
                  FROM sexual_orientation
                 ORDER BY orientation_id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new SexualOrientation
                {
                    OrientationId = rdr.GetInt32(0),
                    Description   = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(SexualOrientation entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE sexual_orientation
                   SET description = @description
                 WHERE orientation_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id",          entity.OrientationId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró sexual_orientation con id = {entity.OrientationId} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM sexual_orientation
                 WHERE orientation_id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró sexual_orientation con id = {id} para eliminar.");
        }
    }
}
