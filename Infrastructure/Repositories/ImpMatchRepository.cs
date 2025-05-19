using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using Npgsql;
using SGCI_app.domain.Ports;
using SGCI_app.infrastructure.postgres;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpMatchRepository : IGenericRepository<Match>, IMatchRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpMatchRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(Match entity)
        {
            throw new NotImplementedException();
        }

        public void Create(System.Text.RegularExpressions.Match entity)
        {
            throw new NotImplementedException();
        }

        public void CreateMatch(int user1Id, int user2Id)
        {
            var conn = _conexion.ObtenerConexion();
            using var tx = conn.BeginTransaction();

            try
            {
                int matchId;

                // 1. Insertar en la tabla `match` y obtener el `match_id` generado
                using (var cmd = new NpgsqlCommand(@"
            INSERT INTO match (user1_id, user2_id)
            VALUES (@u1, @u2)
            RETURNING match_id;
        ", conn, tx))
                {
                    cmd.Parameters.AddWithValue("u1", user1Id);
                    cmd.Parameters.AddWithValue("u2", user2Id);
                    matchId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2. Insertar en `user_match` para ambos usuarios
                using (var cmd = new NpgsqlCommand(@"
            INSERT INTO user_match (user_id, match_id)
            VALUES (@u1, @mid), (@u2, @mid);
        ", conn, tx))
                {
                    cmd.Parameters.AddWithValue("u1", user1Id);
                    cmd.Parameters.AddWithValue("u2", user2Id);
                    cmd.Parameters.AddWithValue("mid", matchId);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }

        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsMatchBetween(int userAId, int userBId)
        {
            var conn = _conexion.ObtenerConexion();

            using var cmd = new NpgsqlCommand(@"
        SELECT COUNT(*) 
        FROM match 
        WHERE (user1_id = @userA AND user2_id = @userB)
           OR (user1_id = @userB AND user2_id = @userA);
    ", conn);

            cmd.Parameters.AddWithValue("userA", userAId);
            cmd.Parameters.AddWithValue("userB", userBId);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }


        public List<Match> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Match entity)
        {
            throw new NotImplementedException();
        }

        public void Update(System.Text.RegularExpressions.Match entity)
        {
            throw new NotImplementedException();
        }
    }
}