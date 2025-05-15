using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGCI_app.domain.Ports
{
    public interface IGenericRepository<T>
    {
        List<T> GetAll();
        void Create(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}