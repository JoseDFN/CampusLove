using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using Npgsql;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpAppUserRepository : IGenericRepository<AppUser>, IAppUserRepository
    {
        private readonly string _connectionString;

        public ImpAppUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Create(AppUser entity)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = @"INSERT INTO app_user (name, age, email, password_hash, gender_id, user_type_id) VALUES (@name, @age, @email, @password_hash, @gender_id, @user_type_id)";
                    cmd.Parameters.AddWithValue("@name", entity.Name is null ? DBNull.Value : entity.Name);
                    cmd.Parameters.AddWithValue("@age", entity.Age);
                    cmd.Parameters.AddWithValue("@email", entity.Email is null ? DBNull.Value : entity.Email);
                    cmd.Parameters.AddWithValue("@password_hash", entity.PasswordHash is null ? DBNull.Value : entity.PasswordHash);
                    cmd.Parameters.AddWithValue("@gender_id", entity.GenderId);
                    cmd.Parameters.AddWithValue("@user_type_id", entity.UserTypeId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<AppUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(AppUser entity)
        {
            throw new NotImplementedException();
        }
    }
}