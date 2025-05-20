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
    public class ImpInteractionCreditsRepository : IGenericRepository<InteractionCredits>, IInteractionCreditsRepository
    {
        private readonly ConexionSingleton _conexion;

        public ImpInteractionCreditsRepository(string connectionString)
        {
            _conexion = ConexionSingleton.Instancia(connectionString);
        }

        public void Create(InteractionCredits entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<InteractionCredits> GetAll()
        {
            throw new NotImplementedException();
        }

        public void ResetLikesAvailable()
        {
            const string sql = @"
                UPDATE interaction_credits
                   SET likes_available = @likes;
            ";
            int newValue = 5;

            var conn = _conexion.ObtenerConexion();
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("likes", newValue);

            cmd.ExecuteNonQuery();
        }

        public DateTime GetLastEventDate(string eventName)
        {
            const string sql = @"
        SELECT last_run
          FROM system_events
         WHERE event_name = @event;
    ";

            var conn = _conexion.ObtenerConexion();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("event", eventName);
            var result = cmd.ExecuteScalar();
            return result == null
                ? DateTime.MinValue
                : ((DateTime)result).Date;
        }

        public void UpdateEventDate(string eventName, DateTime date)
        {
            const string sql = @"
        UPDATE system_events
           SET last_run = @date
         WHERE event_name = @event;
    ";

            var conn = _conexion.ObtenerConexion();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("event", eventName);
            cmd.Parameters.AddWithValue("date", date.Date);
            cmd.ExecuteNonQuery();
        }


        public void Update(InteractionCredits entity)
        {
            throw new NotImplementedException();
        }
    }
}