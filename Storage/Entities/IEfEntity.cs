namespace EntityFramework.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public interface IEfEntity : IEntity, IAuditableEntity
    {
    }

    public interface IAuditableEntity
    {
        DateTime ModifiedOn { get; set; }

        DateTime CreatedOn { get; set; }
    }

    public interface ISoftDelete
    {
        DateTime? DeletedOn { get; set; }
        bool IsDeleted { get; set; }
    }
}
