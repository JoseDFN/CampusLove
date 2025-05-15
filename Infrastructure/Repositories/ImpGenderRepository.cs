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
    public class ImpGenderRepository : IGenericRepository<Gender>, IGenderRepository
    {
        private readonly string _connectionString;

        public ImpGenderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Gender entity)
        {
            const string sql = @"
                INSERT INTO gender (description)
                VALUES (@description);
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            const string sql = @"
                DELETE FROM gender
                 WHERE gender_id = @id;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Gender> GetAll()
        {
            const string sql = @"SELECT career_id, name FROM career;";

            var list = new List<Gender>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Gender
                {
                    GenderId = rdr.GetInt32(0),
                    Description     = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(Gender entity)
        {
            const string sql = @"
                UPDATE gender
                   SET description = @description
                 WHERE gender_id = @id;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id",   entity.GenderId);

            cmd.ExecuteNonQuery();
        }
    }
}