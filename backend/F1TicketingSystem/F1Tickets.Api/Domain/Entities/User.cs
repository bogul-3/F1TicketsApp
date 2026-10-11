using F1Tickets.Api.Domain.Common;
using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Domain.Lookups;

namespace F1Tickets.Api.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; } 
        public string Surname { get; set; } 
        public string Email { get; set; } 
        public string Password { get; set; } 
        public string Address { get; set; } 
        public string PostalCode { get; set; } 
        public UserRole Role { get; set; }

        // RELATIONSHIPS
        public List<Ticket> Tickets { get; set; } 
        public Guid CityId { get; set; }
        public City City { get; set; } 
    }
}