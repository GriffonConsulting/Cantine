namespace EntityFramework.Entities
{
    public class Order : IEfEntity
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public decimal ClientAmount { get; set; }
        public decimal CareAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual Client? Client { get; set; }
        public virtual ICollection<OrderContent> OrderContents { get; set; } = new List<OrderContent>();
    }
}
