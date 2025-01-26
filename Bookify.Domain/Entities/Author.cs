

namespace Bookify.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]

    public class Author : BaseEntity
    {

        public int Id { set; get; }
        [MaxLength(100)]
        public string Name { set; get; } = null!;



    }
}
