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
    public class ImpInteractionRepository : IGenericRepository<Interaction>, IInteractionRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpInteractionRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Interaction entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Interaction> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Interaction entity)
        {
            throw new NotImplementedException();
        }

        public void Like(int userFrom, int userTo)
        {
            var conn = _conexion.ObtenerConexion();
            using var tx = conn.BeginTransaction();

            // Decrementar créditos disponibles si > 0 y devolver el restante
            const string updateCredits = @"
                UPDATE interaction_credits
                   SET likes_available = likes_available - 1
                 WHERE user_id         = @from
                   AND on_date         = CURRENT_DATE
                   AND likes_available > 0
                RETURNING likes_available;
            ";

            int? remaining;
            using (var cmd = new NpgsqlCommand(updateCredits, conn, tx))
            {
                cmd.Parameters.AddWithValue("from", userFrom);
                var result = cmd.ExecuteScalar();
                remaining = result == null ? (int?)null : Convert.ToInt32(result);
            }

            if (remaining == null)
                throw new InvalidOperationException("No tienes likes disponibles para hoy.");

            // Insertar interacción "Like"
            const string insertInteraction = @"
                INSERT INTO interaction (source_user_id, target_user_id, interaction_type_id)
                VALUES (@from, @to,
                    (SELECT id FROM interaction_type WHERE description = 'Like')
                );
            ";
            using (var cmd = new NpgsqlCommand(insertInteraction, conn, tx))
            {
                cmd.Parameters.AddWithValue("from", userFrom);
                cmd.Parameters.AddWithValue("to", userTo);
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        public void Dislike(int userFrom, int userTo)
        {
            var conn = _conexion.ObtenerConexion();

            // Insertar interacción "Dislike" sin afectar créditos
            const string insertDislike = @"
                INSERT INTO interaction (source_user_id, target_user_id, interaction_type_id)
                VALUES (@from, @to,
                    (SELECT id FROM interaction_type WHERE description = 'Dislike')
                );
            ";
            using var cmd = new NpgsqlCommand(insertDislike, conn) { CommandType = CommandType.Text };
            cmd.Parameters.AddWithValue("from", userFrom);
            cmd.Parameters.AddWithValue("to", userTo);
            cmd.ExecuteNonQuery();
        }

        public bool HasInteraction(int sourceUserId, int targetUserId)
        {
            var conn = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT EXISTS(
                    SELECT 1 FROM interaction
                     WHERE source_user_id = @source
                       AND target_user_id = @target
                       AND interaction_type_id = 1
                );
            ";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("source", sourceUserId);
            cmd.Parameters.AddWithValue("target", targetUserId);

            var result = cmd.ExecuteScalar();
            // ExecuteScalar should return a boolean for EXISTS, but guard against null
            return result != null && Convert.ToBoolean(result);
        }
    }
}
