using Domain.Client;

namespace EntityFramework.Entities
{
    public class Role : IEfEntity
    {
        public Guid Id { get; set; }
        public ClientRole ClientRole { get; set; }
        public decimal? MealCareAmount { get; set; }
        public decimal? MealCarePercent { get; set; }
        public bool CanOverDraft { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
