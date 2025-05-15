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
    public class ImpUserTypeRepository : IGenericRepository<UserType>, IUserTypeRepository
    {
        private readonly string _connectionString;

        public ImpUserTypeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Create(UserType entity)
        {
            const string sql = @"
                INSERT INTO user_type (description)
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
                DELETE FROM user_type
                 WHERE id = @id;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            cmd.ExecuteNonQuery();
        }

        public List<UserType> GetAll()
        {
            const string sql = @"SELECT id, description FROM user_type;";

            var list = new List<UserType>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(new UserType
                {
                    Id = rdr.GetInt32(0),
                    Description     = rdr.IsDBNull(1) ? null : rdr.GetString(1)
                });
            }

            return list;
        }

        public void Update(UserType entity)
        {
            const string sql = @"
                UPDATE user_type
                   SET description = @description
                 WHERE id = @id;
            ";

            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("description", entity.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("id",   entity.Id);

            cmd.ExecuteNonQuery();
        }
    }
}