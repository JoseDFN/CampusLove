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
    public class ImpUserCareerRepository : IGenericRepository<UserCareer>, IUserCareerRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpUserCareerRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(UserCareer entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO user_career (user_id, career_id)
                VALUES (@userId, @careerId);
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("userId", entity.UserId);
            cmd.Parameters.AddWithValue("careerId", entity.CareerId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se pudo insertar la relación (user_id = {entity.UserId}, career_id = {entity.CareerId}).");
        }

        /// <summary>
        /// Elimina por clave compuesta.
        /// </summary>
        public void Delete(int userId, int careerId)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM user_career
                 WHERE user_id   = @userId
                   AND career_id = @careerId;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("careerId", careerId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró la relación (user_id = {userId}, career_id = {careerId}) para eliminar.");
        }

        /// <summary>
        /// No aplica (clave compuesta). Use Delete(userId, careerId).
        /// </summary>
        public void Delete(int id)
            => throw new NotImplementedException("Use Delete(userId, careerId) para eliminar.");

        public List<UserCareer> GetAll()
        {
            var list = new List<UserCareer>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT user_id, career_id
                  FROM user_career
                 ORDER BY user_id, career_id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserCareer
                {
                    UserId   = rdr.GetInt32(0),
                    CareerId = rdr.GetInt32(1)
                });
            }

            return list;
        }

        /// <summary>
        /// Actualiza la carrera de un usuario (clave compuesta).
        /// </summary>
        public void Update(int userId, int oldCareerId, int newCareerId)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE user_career
                   SET career_id = @newCareerId
                 WHERE user_id   = @userId
                   AND career_id = @oldCareerId;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("oldCareerId", oldCareerId);
            cmd.Parameters.AddWithValue("newCareerId", newCareerId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró la relación (user_id = {userId}, career_id = {oldCareerId}) para actualizar.");
        }

        /// <summary>
        /// Interfaz genérica: no hay forma de saber el oldCareerId, así que no implementado.
        /// </summary>
        public void Update(UserCareer entity)
            => throw new NotImplementedException("Use Update(userId, oldCareerId, newCareerId) para actualizar.");
    }
}
