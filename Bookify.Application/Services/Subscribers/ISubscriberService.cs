namespace Bookify.Application.Services
{
    public interface ISubscriberService
    {
        IQueryable<Subscriber> GetQueryable();
        IQueryable<Subscriber> GetQueryableDetails();
        IQueryable<Subscriber> GetQueryableSubscribersWithRentals();
        Subscriber? GetById(int subscriberId);
        Subscriber GetSubscriberWithSubscriptions(int id);
        IEnumerable<Area> GetActiveAreasByGovernorateId(int gevernorateId);
        IEnumerable<Governorate> GetActiveGovernorates();

        Subscriber Create(Subscriber subscriber, string imagePath, string imageName, string createdById);
        void Update(Subscriber subscriber, string updatedById);
        Subscription RenewSubscription(Subscriber subscriber, string updatedById);
        bool AllowEmail(int subscriberId, string email);
        bool AllowNationalId(int subscriberId, string nationalId);
        bool AllowMobileNumber(int subscriberId, string mobileNumber);
        // Dashboard
        int GetNumberOfActiveSubscribers();
        IEnumerable<KeyValuePairDto> GetSubscribersPerCity();
    }
}
