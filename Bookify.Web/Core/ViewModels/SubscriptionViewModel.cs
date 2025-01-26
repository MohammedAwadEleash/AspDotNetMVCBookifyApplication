namespace Bookify.Web.Core.ViewModels
{
    public class SubscriptionViewModel
    {

        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public DateTime CreatedOn { set; get; }


        public string Status
        {

            get
            {

                return DateTime.Today > EndDate ? SubscriptionStatus.Expired : DateTime.Today < StartDate ? "Not Started" : SubscriptionStatus.Active;

            }
        }
    }
}

