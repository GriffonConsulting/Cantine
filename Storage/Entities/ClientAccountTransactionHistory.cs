namespace EntityFramework.Entities
{
    public class ClientAccountTransactionHistory : IEfEntity
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProviderId { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal ClientNewAmount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual Client? Client { get; set; }
    }
}
