using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CampusLove.Domain.Entities;
using SGCI_app.infrastructure.postgres;
using SGCI_app.domain.Ports;
using CampusLove.Domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpUserTypeRepository : IGenericRepository<UserType>, IUserTypeRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpUserTypeRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(UserType entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO user_type (description)
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
                    $"No se pudo insertar el tipo de usuario (description = '{entity.Description}').");
        }

        public List<UserType> GetAll()
        {
            var list = new List<UserType>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT id, description
                  FROM user_type
                 ORDER BY id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserType
                {
                    Id = rdr.GetInt32(0),
                    Description = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(UserType entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE user_type
                   SET description = @description
                 WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id", entity.Id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró user_type con id = {entity.Id} para actualizar.");
        }

        public void Delete(int id)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM user_type
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
                    $"No se encontró user_type con id = {id} para eliminar.");
        }
    }
}
