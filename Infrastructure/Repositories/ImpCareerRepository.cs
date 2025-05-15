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
    public class ImpCareerRepository : IGenericRepository<Career>, ICareerRepository
    {
        private readonly string _connectionString;

        public ImpCareerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(Career entity)
        {
            const string sql = @"
                INSERT INTO career (name)
                VALUES (@name);
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            const string sql = @"
                DELETE FROM career
                 WHERE career_id = @id;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Career> GetAll()
        {
            const string sql = @"SELECT career_id, name FROM career;";

            var list = new List<Career>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new Career
                {
                    CareerId = rdr.GetInt32(0),
                    Name     = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(Career entity)
        {
            const string sql = @"
                UPDATE career
                   SET name = @name
                 WHERE career_id = @id;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", entity.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id",   entity.CareerId);

            cmd.ExecuteNonQuery();
        }
    }
}