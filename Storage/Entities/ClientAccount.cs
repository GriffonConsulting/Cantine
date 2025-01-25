namespace EntityFramework.Entities
{
    public class ClientAccount : IEfEntity
    {
        public Guid Id { get; set; }
        public Guid ClientId {  get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual Client? Client { get; set; }
    }
}
