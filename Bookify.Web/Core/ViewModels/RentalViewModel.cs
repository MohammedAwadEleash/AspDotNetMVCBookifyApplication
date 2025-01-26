namespace Bookify.Web.Core.ViewModels
{
    public class RentalViewModel
    {

        public int Id { get; set; }
        public SubscriberViewModel? subscriber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime CreatedOn { set; get; }
        public bool PenaltyPaid { get; set; }
        ///   public bool IsDeleted { get; set; }


        public IEnumerable<RentalCopyViewModel> RentalCopies = new List<RentalCopyViewModel>();

        public int TotalDelayInDays
        {


            get
            {

                return RentalCopies.Sum(rc => rc.DelayInDays);

            }

        }

        public int NumberOfCopies
        {
            get
            {
                return RentalCopies.Count();
            }
        }


    }
}
