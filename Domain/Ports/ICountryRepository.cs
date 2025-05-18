using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        void CrearCountry(Country country);
        void ActualizarCountry(Country country);
        void EliminarCountry(int id);
        Country ObtenerCountry(int id);
        IEnumerable<Country> ObtenerTodos();
    }
}