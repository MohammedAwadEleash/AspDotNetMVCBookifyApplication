namespace Bookify.Domain.Entities
{
    [Index(nameof(Name), nameof(GovernorateId), IsUnique = true)]
    public class Area : BaseEntity
    {


        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { set; get; } = null!;

        public int GovernorateId { set; get; }

        public Governorate? Governorate { set; get; }


    }
}
