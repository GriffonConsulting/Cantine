using Domain.Client;

namespace EntityFramework.Entities
{
    public class MealCare : IEfEntity
    {
        public Guid Id { get; set; }
        public ClientRole ClientRole { get; set; }
        public decimal Amount { get; set; }
        public decimal Percent { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
