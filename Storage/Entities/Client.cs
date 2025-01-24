using Domain.Client;

namespace EntityFramework.Entities
{
    public class Client : IEfEntity
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public ClientRole ClientRole {  get; set; } 
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
