using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using Npgsql;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpUserCareerRepository : IGenericRepository<UserCareer>, IUserCareerRepository
    {
        private readonly string _connectionString;

        public ImpUserCareerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(UserCareer entity)
        {
            const string sql = @"
            INSERT INTO user_career (user_id, career_id)
            VALUES (@userId, @careerId);
        ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userId", entity.UserId);
            cmd.Parameters.AddWithValue("careerId", entity.CareerId);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int userId, int careerId)
        {
            const string sql = @"
            DELETE FROM user_career
             WHERE user_id   = @userId
               AND career_id = @careerId;
        ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("careerId", careerId);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserCareer> GetAll()
        {
            const string sql = "SELECT user_id, career_id FROM user_career;";

            var list = new List<UserCareer>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserCareer
                {
                    UserId = rdr.GetInt32(0),
                    CareerId = rdr.GetInt32(1)
                });
            }

            return list;
        }

        public void Update(int userId, int oldCareerId, int newCareerId)
        {
            const string sql = @"
            UPDATE user_career
               SET career_id = @newCareerId
             WHERE user_id   = @userId
               AND career_id = @oldCareerId;
        ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("userId", userId);
            cmd.Parameters.AddWithValue("oldCareerId", oldCareerId);
            cmd.Parameters.AddWithValue("newCareerId", newCareerId);
            cmd.ExecuteNonQuery();
        }

        public void Update(UserCareer entity)
        {
            throw new NotImplementedException();
        }
    }
}