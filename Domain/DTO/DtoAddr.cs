using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLove.Domain.DTO
{
    public class DtoAddr
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? PostalCode { get; set; }
        public int CityId { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}