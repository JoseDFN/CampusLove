using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Entities;
using CampusLove.Domain.Ports;
using Npgsql;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpAppUserRepository : IGenericRepository<DtoAppUser>, IAppUserRepository
    {
        private readonly string _connectionString;

        public ImpAppUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Create(DtoAppUser dto)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand(@"
                CALL sp_create_app_user(
                    p_name         => @name,
                    p_age          => @age,
                    p_email        => @email,
                    p_password_hash=> @password_hash,
                    p_gender_id    => @gender_id,
                    p_user_type_id => @user_type_id,
                    p_street       => @street,
                    p_building_number => @building_number,
                    p_postal_code  => @postal_code,
                    p_city_id      => @city_id,
                    p_additional_info => @additional_info,
                    p_orientation_id  => @orientation_id,
                    p_min_age       => @min_age,
                    p_max_age       => @max_age,
                    p_profile_text  => @profile_text
                );
            ", conn);


            // app_user
            cmd.Parameters.AddWithValue("name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("age", dto.Age);
            cmd.Parameters.AddWithValue("email", dto.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("password_hash", dto.PasswordHash ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("gender_id", dto.GenderId);
            cmd.Parameters.AddWithValue("user_type_id", dto.UserTypeId);

            // address
            cmd.Parameters.AddWithValue("street", dto.Address.Street ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("building_number", dto.Address.BuildingNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("postal_code", dto.Address.PostalCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("city_id", dto.Address.CityId);
            cmd.Parameters.AddWithValue("additional_info", dto.Address.AdditionalInfo ?? (object)DBNull.Value);

            // preference
            cmd.Parameters.AddWithValue("orientation_id", dto.UserProfile.Preference.OrientationId);
            cmd.Parameters.AddWithValue("min_age", dto.UserProfile.Preference.MinAge);
            cmd.Parameters.AddWithValue("max_age", dto.UserProfile.Preference.MaxAge);

            // user_profile
            cmd.Parameters.AddWithValue("profile_text", dto.UserProfile.ProfileText ?? (object)DBNull.Value);

            // Ejecuta el PROCEDURE (sin retorno)
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            // 1) Obtener los FK para eliminarlos en orden
            int? addressId = null, preferenceId = null;
            using (var cmd = new NpgsqlCommand(@"
            SELECT address_id, preference_id
              FROM user_profile
             WHERE user_id = @user_id;
        ", conn, tx))
            {
                cmd.Parameters.AddWithValue("user_id", id);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    addressId = reader.IsDBNull(0) ? null : reader.GetInt32(0);
                    preferenceId = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                }
                reader.Close();
            }

            // 2) Borrar de user_profile
            using (var cmd = new NpgsqlCommand(
                "DELETE FROM user_profile WHERE user_id = @user_id;", conn, tx))
            {
                cmd.Parameters.AddWithValue("user_id", id);
                cmd.ExecuteNonQuery();
            }

            // 3) Borrar preference (si lo había)
            if (preferenceId.HasValue)
            {
                using var cmd = new NpgsqlCommand(
                    "DELETE FROM preference WHERE preference_id = @pref_id;", conn, tx);
                cmd.Parameters.AddWithValue("pref_id", preferenceId.Value);
                cmd.ExecuteNonQuery();
            }

            // 4) Borrar address (si lo había)
            if (addressId.HasValue)
            {
                using var cmd = new NpgsqlCommand(
                    "DELETE FROM address WHERE id = @addr_id;", conn, tx);
                cmd.Parameters.AddWithValue("addr_id", addressId.Value);
                cmd.ExecuteNonQuery();
            }

            // 5) Borrar app_user
            using (var cmd = new NpgsqlCommand(
                "DELETE FROM app_user WHERE user_id = @user_id;", conn, tx))
            {
                cmd.Parameters.AddWithValue("user_id", id);
                var rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException($"No se encontró usuario con id = {id} para eliminar.");
            }

            tx.Commit();
        }

        public List<DtoAppUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(DtoAppUser entity)
        {
            throw new NotImplementedException();
        }

        public void update(int id, DtoAppUser dto)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand("CALL sp_update_app_user(@user_id, @name, @age, @email, @password_hash, @gender_id, @user_type_id);", conn);
            cmd.Parameters.AddWithValue("user_id", id);
            cmd.Parameters.AddWithValue("name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("age", dto.Age);
            cmd.Parameters.AddWithValue("email", dto.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("password_hash", dto.PasswordHash ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("gender_id", dto.GenderId);
            cmd.Parameters.AddWithValue("user_type_id", dto.UserTypeId);

            var affected = cmd.ExecuteNonQuery();
            if (affected == 0)
                throw new InvalidOperationException($"No se encontró usuario con id = {id} para actualizar.");
        }

    }
}