using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using SGCI_app.domain.Ports;
using SGCI_app.infrastructure.postgres;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpUserInterestsRepository : IGenericRepository<UserInterest>, IUserInterestRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpUserInterestsRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(UserInterest entity)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                INSERT INTO user_interest (user_id, interest_id)
                VALUES (@userId, @interestId);
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("userId", entity.UserId);
            cmd.Parameters.AddWithValue("interestId", entity.InterestId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se pudo insertar la relación (user_id = {entity.UserId}, interest_id = {entity.InterestId}).");
        }

        /// <summary>
        /// No aplica (clave compuesta). Use Delete(userId, interestId).
        /// </summary>
        public void Delete(int id)
            => throw new NotImplementedException("Use Delete(userId, interestId) para eliminar.");

        /// <summary>
        /// Elimina por clave compuesta.
        /// </summary>
        public void Delete(int userId, int interestId)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                DELETE FROM user_interest
                 WHERE user_id     = @userId
                   AND interest_id = @interestId;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("interestId", interestId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró la relación (user_id = {userId}, interest_id = {interestId}) para eliminar.");
        }

        public List<UserInterest> GetAll()
        {
            var list = new List<UserInterest>();
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT user_id, interest_id
                  FROM user_interest
                 ORDER BY user_id, interest_id;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserInterest
                {
                    UserId     = rdr.GetInt32(0),
                    InterestId = rdr.GetInt32(1)
                });
            }

            return list;
        }

        /// <summary>
        /// Interfaz genérica: no hay forma de saber el oldInterestId, use Update(userId, oldInterestId, newInterestId).
        /// </summary>
        public void Update(UserInterest entity)
            => throw new NotImplementedException("Use Update(userId, oldInterestId, newInterestId) para actualizar.");

        /// <summary>
        /// Actualiza el interés de un usuario (clave compuesta).
        /// </summary>
        public void Update(int userId, int oldInterestId, int newInterestId)
        {
            var connection = _conexion.ObtenerConexion();
            const string sql = @"
                UPDATE user_interest
                   SET interest_id = @newInterestId
                 WHERE user_id     = @userId
                   AND interest_id = @oldInterestId;
            ";

            using var cmd = new NpgsqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("oldInterestId", oldInterestId);
            cmd.Parameters.AddWithValue("newInterestId", newInterestId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException(
                    $"No se encontró la relación (user_id = {userId}, interest_id = {oldInterestId}) para actualizar.");
        }
    }
}
