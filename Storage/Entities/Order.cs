namespace EntityFramework.Entities
{
    public class Order : IEfEntity
    {
        public Guid Id { get; set; }
        public decimal ClientAmount { get; set; }
        public decimal CareAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
