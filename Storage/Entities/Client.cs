namespace EntityFramework.Entities
{
    public class Client : IEfEntity
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public Guid RoleId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual Role? Role { get; set; }
    }
}
