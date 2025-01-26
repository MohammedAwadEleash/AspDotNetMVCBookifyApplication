namespace Bookify.Domain.Entities
{
    public class Subscription
    {
        public int Id { set; get; }

        public int SubscriberId { set; get; }

        public Subscriber? subscriber { get; set; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }

        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        public DateTime CreatedOn { set; get; } = DateTime.Now;



    }
}
