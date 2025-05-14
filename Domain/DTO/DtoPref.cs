using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.DTO
{
    public class DtoPref
    {
        public int Id { get; set; }
        public int OrientationId { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
    }
}