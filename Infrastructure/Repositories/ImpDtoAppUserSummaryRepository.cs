using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using CampusLove.Domain.DTO;
using CampusLove.Domain.Ports;
using SGCI_app.infrastructure.postgres;
using SGCI_app.domain.Ports;

namespace CampusLove.Infrastructure.Repositories
{
    public class ImpDtoAppUserSummaryRepository : IGenericRepository<DtoAppUserSummary>, IDtoAppUserSummaryRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpDtoAppUserSummaryRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(DtoAppUserSummary entity)
            => throw new NotImplementedException("Imposible crear un summary directamente. Use GetAll para leer los resúmenes.");

        public void Delete(int id)
            => throw new NotImplementedException("No aplicable: los summaries son de solo lectura. No se permite eliminar.");

        public void Update(DtoAppUserSummary entity)
            => throw new NotImplementedException("No aplicable: los summaries son de solo lectura. No se permite actualizar.");

        public List<DtoAppUserSummary> GetAll()
        {
            var results = new List<DtoAppUserSummary>();
            var conn = _conexion.ObtenerConexion();
            const string sql = @"
                SELECT
                  u.user_id,
                  u.name,
                  u.age,
                  g.name       AS gender,
                  cty.name     AS city,
                  COALESCE(array_agg(DISTINCT cr.name)      FILTER (WHERE cr.name      IS NOT NULL), '{}') AS careers,
                  COALESCE(array_agg(DISTINCT i.description) FILTER (WHERE i.description IS NOT NULL), '{}') AS interests
                FROM app_user u
                JOIN gender g          ON g.gender_id       = u.gender_id
                JOIN user_profile up   ON up.user_id        = u.user_id
                JOIN address a         ON a.id              = up.address_id
                JOIN city cty          ON cty.id            = a.city_id
                LEFT JOIN user_career uc ON uc.user_id       = u.user_id
                LEFT JOIN career cr      ON cr.career_id     = uc.career_id
                LEFT JOIN user_interest ui ON ui.user_id      = u.user_id
                LEFT JOIN interest i       ON i.interest_id   = ui.interest_id
                GROUP BY u.user_id, u.name, u.age, g.name, cty.name
                ORDER BY u.user_id;
            ";

            using var cmd = new NpgsqlCommand(sql, conn)
            {
                CommandType = CommandType.Text
            };
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                results.Add(new DtoAppUserSummary
                {
                    UserId    = rdr.GetInt32(rdr.GetOrdinal("user_id")),
                    Name      = rdr.GetString  (rdr.GetOrdinal("name")),
                    Age       = rdr.GetInt32   (rdr.GetOrdinal("age")),
                    Gender    = rdr.GetString  (rdr.GetOrdinal("gender")),
                    City      = rdr.GetString  (rdr.GetOrdinal("city")),
                    Careers   = rdr.GetFieldValue<string[]>(rdr.GetOrdinal("careers")).ToList(),
                    Interests = rdr.GetFieldValue<string[]>(rdr.GetOrdinal("interests")).ToList()
                });
            }

            return results;
        }
    }
}
