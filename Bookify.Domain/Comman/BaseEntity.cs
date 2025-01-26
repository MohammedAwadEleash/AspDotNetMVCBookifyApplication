
using Bookify.Domain.Entities;

namespace Bookify.Domain.Comman

{
    public class BaseEntity
    {

        public bool IsDeleted { get; set; }

        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { set; get; } = DateTime.Now;



        public string? LastUpdatedById { get; set; }
        public ApplicationUser? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedOn { set; get; }





    }
}
