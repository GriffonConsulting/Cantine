using Domain.Client;

namespace EntityFramework.Entities
{
    public class Product : IEfEntity
    {
        public Guid Id { get; set; }
        public required string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
