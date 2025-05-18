using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLove.Domain.Entities;
using SGCI_app.domain.Ports;

namespace CampusLove.Domain.Ports
{
    public interface IRegionRepository : IGenericRepository<Region>
    {
        void CrearRegion(Region region);
        void ActualizarRegion(Region region);
        void EliminarRegion(int id);
        Region ObtenerRegion(int id);
        IEnumerable<Region> ObtenerTodos();
    }
}