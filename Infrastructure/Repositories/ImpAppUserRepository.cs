using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Ports;
using SGCI_app.infrastructure.postgres;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpAppUserRepository : IGenericRepository<DtoAppUser>, IAppUserRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpAppUserRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        // --------------------------------------------------
        // IGenericRepository<DtoAppUser>
        // --------------------------------------------------

        public void Create(DtoAppUser entity)
        {
            // Delegamos al método create(...) y descartamos el id
            create(entity);
        }

        public List<DtoAppUser> GetAll()
        {
            var list = new List<DtoAppUser>();
            var conn = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT
                    u.user_id, u.name, u.age, u.email, u.password_hash, u.gender_id, u.user_type_id,
                    a.id          AS addr_id,      a.street,      a.building_number, 
                    a.postal_code, a.city_id,      a.additional_info,
                    p.preference_id, p.orientation_id, p.min_age, p.max_age,
                    up.profile_text, up.verified, up.status, up.updated_at
                FROM app_user u
                JOIN user_profile up   ON u.user_id     = up.user_id
                JOIN address a          ON up.address_id = a.id
                JOIN preference p       ON up.preference_id = p.preference_id
                ORDER BY u.user_id;
            ";

            using var cmd = new NpgsqlCommand(sql, conn) { CommandType = CommandType.Text };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var dto = new DtoAppUser
                {
                    UserId = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1),
                    Age = rdr.GetInt32(2),
                    Email = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3),
                    PasswordHash = rdr.IsDBNull(4) ? string.Empty : rdr.GetString(4),
                    GenderId = rdr.GetInt32(5),
                    UserTypeId = rdr.GetInt32(6),
                    Address = rdr.IsDBNull(7) ? new DtoAddr() : new DtoAddr
                    {
                        Id = rdr.GetInt32(7),
                        Street = rdr.IsDBNull(8) ? string.Empty : rdr.GetString(8),
                        BuildingNumber = rdr.IsDBNull(9) ? string.Empty : rdr.GetString(9),
                        PostalCode = rdr.IsDBNull(10) ? string.Empty : rdr.GetString(10),
                        CityId = rdr.GetInt32(11),
                        AdditionalInfo = rdr.IsDBNull(12) ? string.Empty : rdr.GetString(12),
                    },
                    UserProfile = rdr.IsDBNull(13) ? new DtoUserProf() : new DtoUserProf
                    {
                        UserId = rdr.GetInt32(0),
                        PreferenceId = rdr.GetInt32(13),
                        Preference = new DtoPref
                        {
                            Id = rdr.GetInt32(13),
                            OrientationId = rdr.GetInt32(14),
                            MinAge = rdr.GetInt32(15),
                            MaxAge = rdr.GetInt32(16)
                        },
                        ProfileText = rdr.IsDBNull(17) ? string.Empty : rdr.GetString(17),
                        Verified = rdr.GetBoolean(18),
                        Status = rdr.IsDBNull(19) ? string.Empty : rdr.GetString(19),
                        UpdatedAt = rdr.GetDateTime(20)
                    }
                };
                list.Add(dto);
            }
            return list;
        }

        public void Update(DtoAppUser entity)
        {
            // Delegamos al método update(...)
            update(entity.UserId, entity);
        }

        public void Delete(int id)
        {
            var conn = _conexion.ObtenerConexion();
            using var tx = conn.BeginTransaction();

            // 1) Obtener los FKs
            int? addrId = null, prefId = null;
            using (var cmd = new NpgsqlCommand(@"
                SELECT address_id, preference_id
                  FROM user_profile
                 WHERE user_id = @uid;
            ", conn, tx))
            {
                cmd.Parameters.AddWithValue("uid", id);
                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    addrId = rdr.IsDBNull(0) ? null : rdr.GetInt32(0);
                    prefId = rdr.IsDBNull(1) ? null : rdr.GetInt32(1);
                }
            }

            // 2) Borrar user_profile
            using (var cmd = new NpgsqlCommand(
                "DELETE FROM user_profile WHERE user_id = @uid;", conn, tx))
            {
                cmd.Parameters.AddWithValue("uid", id);
                cmd.ExecuteNonQuery();
            }

            // 3) Borrar preference
            if (prefId.HasValue)
            {
                using var cmd = new NpgsqlCommand(
                    "DELETE FROM preference WHERE preference_id = @pid;", conn, tx);
                cmd.Parameters.AddWithValue("pid", prefId.Value);
                cmd.ExecuteNonQuery();
            }

            // 4) Borrar address
            if (addrId.HasValue)
            {
                using var cmd = new NpgsqlCommand(
                    "DELETE FROM address WHERE id = @aid;", conn, tx);
                cmd.Parameters.AddWithValue("aid", addrId.Value);
                cmd.ExecuteNonQuery();
            }

            // 5) Borrar app_user
            using (var cmd = new NpgsqlCommand(
                "DELETE FROM app_user WHERE user_id = @uid;", conn, tx))
            {
                cmd.Parameters.AddWithValue("uid", id);
                var rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException($"No se encontró app_user con id = {id} para eliminar.");
            }

            tx.Commit();
        }

        // --------------------------------------------------
        // IAppUserRepository
        // --------------------------------------------------

        public int create(DtoAppUser dto)
        {
            var conn = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT fn_create_app_user(
                    @name, @age, @email, @password_hash, @gender_id,
                    @street, @building_number, @postal_code, @city_id, @additional_info,
                    @orientation_id, @min_age, @max_age, @profile_text
                );
            ";

            using var cmd = new NpgsqlCommand(sql, conn) { CommandType = CommandType.Text };
            // Parámetros de app_user
            cmd.Parameters.AddWithValue("name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("age", dto.Age);
            cmd.Parameters.AddWithValue("email", dto.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("password_hash", dto.PasswordHash ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("gender_id", dto.GenderId);
            // Parámetros de address
            cmd.Parameters.AddWithValue("street", dto.Address.Street ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("building_number", dto.Address.BuildingNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("postal_code", dto.Address.PostalCode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("city_id", dto.Address.CityId);
            cmd.Parameters.AddWithValue("additional_info", dto.Address.AdditionalInfo ?? (object)DBNull.Value);
            // Parámetros de preference
            cmd.Parameters.AddWithValue("orientation_id", dto.UserProfile.Preference.OrientationId);
            cmd.Parameters.AddWithValue("min_age", dto.UserProfile.Preference.MinAge);
            cmd.Parameters.AddWithValue("max_age", dto.UserProfile.Preference.MaxAge);
            // Parámetros de user_profile
            cmd.Parameters.AddWithValue("profile_text", dto.UserProfile.ProfileText ?? (object)DBNull.Value);

            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public void update(int id, DtoAppUser dto)
        {
            var conn = _conexion.ObtenerConexion();
            const string sql = @"
                CALL sp_update_app_user(
                    @user_id, @name, @age, @email, @password_hash, @gender_id
                );
            ";
            using var cmd = new NpgsqlCommand(sql, conn) { CommandType = CommandType.Text };
            cmd.Parameters.AddWithValue("user_id", id);
            cmd.Parameters.AddWithValue("name", dto.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("age", dto.Age);
            cmd.Parameters.AddWithValue("email", dto.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("password_hash", dto.PasswordHash ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("gender_id", dto.GenderId);

            var affected = cmd.ExecuteNonQuery();
            if (affected == 0)
                throw new InvalidOperationException($"No se encontró app_user con id = {id} para actualizar.");
        }
        
        public DtoAppUser ObtenerUsuarioPorEmail(string email)
        {
            var conn = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT
                    u.user_id, u.name, u.age, u.email, u.password_hash, u.gender_id, u.user_type_id,
                    a.id AS addr_id, a.street, a.building_number, a.postal_code, a.city_id, a.additional_info,
                    p.preference_id, p.orientation_id, p.min_age, p.max_age,
                    up.profile_text, up.verified, up.status, up.updated_at
                FROM app_user u
                LEFT JOIN user_profile up ON u.user_id = up.user_id
                LEFT JOIN address a ON up.address_id = a.id
                LEFT JOIN preference p ON up.preference_id = p.preference_id
                WHERE u.email = @correo;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("correo", email);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new DtoAppUser
                {
                    UserId = rdr.GetInt32(0),
                    Name = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1),
                    Age = rdr.GetInt32(2),
                    Email = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3),
                    PasswordHash = rdr.IsDBNull(4) ? string.Empty : rdr.GetString(4),
                    GenderId = rdr.GetInt32(5),
                    UserTypeId = rdr.GetInt32(6),
                    Address = rdr.IsDBNull(7) ? new DtoAddr() : new DtoAddr
                    {
                        Id = rdr.GetInt32(7),
                        Street = rdr.IsDBNull(8) ? string.Empty : rdr.GetString(8),
                        BuildingNumber = rdr.IsDBNull(9) ? string.Empty : rdr.GetString(9),
                        PostalCode = rdr.IsDBNull(10) ? string.Empty : rdr.GetString(10),
                        CityId = rdr.GetInt32(11),
                        AdditionalInfo = rdr.IsDBNull(12) ? string.Empty : rdr.GetString(12),
                    },
                    UserProfile = rdr.IsDBNull(13) ? new DtoUserProf() : new DtoUserProf
                    {
                        UserId = rdr.GetInt32(0),
                        PreferenceId = rdr.GetInt32(13),
                        Preference = new DtoPref
                        {
                            Id = rdr.GetInt32(13),
                            OrientationId = rdr.GetInt32(14),
                            MinAge = rdr.GetInt32(15),
                            MaxAge = rdr.GetInt32(16),
                        },
                        ProfileText = rdr.IsDBNull(17) ? string.Empty : rdr.GetString(17),
                        Verified = rdr.GetBoolean(18),
                        Status = rdr.IsDBNull(19) ? string.Empty : rdr.GetString(19),
                        UpdatedAt = rdr.GetDateTime(20)
                    }
                };
            }
            return null!;
        }

    }
}
