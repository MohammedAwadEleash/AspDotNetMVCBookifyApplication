namespace Bookify.Application.Services
{
    internal class SubscriberService : ISubscriberService
    {

        private readonly IUnitOfWork _unitOfWork;

        public SubscriberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IQueryable<Subscriber> GetQueryable()
        {

            var query = _unitOfWork.Subscribers.GetQueryable();        // query : subscribers still in database it return only reference
            return query;


        }

        public IQueryable<Subscriber> GetQueryableDetails()
        {
            var query = _unitOfWork.Subscribers.GetQueryable()
                .Include(s => s.Governorate)
                .Include(s => s.Area)
                .Include(s => s.Subscriptions)
                .Include(s => s.Rentals.OrderByDescending(r => r.StartDate))
                .ThenInclude(r => r.RentalCopies);
            return query;

        }

        public Subscriber? GetById(int subscriberId)
        {

            return _unitOfWork.Subscribers.GetById(subscriberId);
        }
        public Subscriber GetSubscriberWithSubscriptions(int id)
        {
            var subscriber = _unitOfWork.Subscribers.Find(s => s.Id == id, s => s.Include(s => s.Subscriptions));

            return subscriber!;
        }

        public IEnumerable<Area> GetActiveAreasByGovernorateId(int gevernorateId)
        {

            var areas = _unitOfWork.Areas.FindAll(
                predicate: a => a.GovernorateId == gevernorateId && !a.IsDeleted,
                orderBy: a => a.Name,
                orderByDirection: OrderBy.Ascending);

            return areas;
        }
        public IEnumerable<Governorate> GetActiveGovernorates()
        {
            var governorates = _unitOfWork.Governorates.FindAll(predicate: g => !g.IsDeleted, orderBy: g => g.Name, orderByDirection: OrderBy.Ascending);

            return governorates;
        }
        public Subscriber Create(Subscriber subscriber, string imagePath, string imageName, string createdById)
        {
            subscriber.ImageUrl = $"{imagePath}/{imageName}";
            subscriber.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            subscriber.CreatedById = createdById;


            var subscription = new Subscription()
            {
                CreatedById = subscriber.CreatedById,
                CreatedOn = subscriber.CreatedOn,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };


            subscriber.Subscriptions.Add(subscription);
            _unitOfWork.Subscribers.Add(subscriber);
            _unitOfWork.Complete();

            return subscriber;

        }
        public void Update(Subscriber subscriber, string updatedById)
        {
            subscriber.LastUpdatedById = updatedById;
            subscriber.LastUpdatedOn = DateTime.Now;
            _unitOfWork.Complete();

        }

        public Subscription RenewSubscription(Subscriber subscriber, string createdById)
        {
            var lastSubscription = subscriber.Subscriptions.Last();




            var startDate = lastSubscription.EndDate < DateTime.Today
                ? DateTime.Today : lastSubscription.EndDate.AddDays(1);


            var newsubscription = new Subscription()
            {

                CreatedById = createdById,
                CreatedOn = DateTime.Now,
                StartDate = startDate,
                EndDate = startDate.AddYears(1)


            };
            subscriber.Subscriptions.Add(newsubscription);
            _unitOfWork.Complete();
            return newsubscription;
        }

        public bool AllowEmail(int subscriberId, string email)
        {


            var subscriber = _unitOfWork.Subscribers.Find(s => s.Email == email);

            var isValid = subscriber is null || subscriber.Id == subscriberId;

            return isValid;
        }

        public bool AllowNationalId(int subscriberId, string nationalId)
        {
            var subscriber = _unitOfWork.Subscribers.Find(s => s.NationalId == nationalId);

            var isValid = subscriber is null || subscriber.Id == subscriberId;

            return isValid;
        }

        public bool AllowMobileNumber(int subscriberId, string mobileNumber)
        {
            var subscriber = _unitOfWork.Subscribers.Find(s => s.MobileNumber == mobileNumber);

            var isValid = subscriber is null || subscriber.Id == subscriberId;

            return isValid;
        }

        public int GetNumberOfActiveSubscribers()
        {

            return _unitOfWork.Subscribers.Count(s => !s.IsDeleted);
        }

        public IEnumerable<KeyValuePairDto> GetSubscribersPerCity()
        {
            var data = _unitOfWork.Subscribers.GetQueryable()
                          .Include(s => s.Governorate)
                          .Where(s => !s.IsDeleted)

                          .GroupBy(s => new
                          {
                              GovernorateName = s.Governorate!.Name,


                          }).Select(g => new KeyValuePairDto
                         (
                               g.Key.GovernorateName,
                               g.Count().ToString()
                          )
                          ).ToList();

            return data;
        }

        public IQueryable<Subscriber> GetQueryableSubscribersWithRentals()
        {
            var query = _unitOfWork.Subscribers.GetQueryable()
              .Include(s => s.Subscriptions)
              .Include(s => s.Rentals)
              .ThenInclude(r => r.RentalCopies);
            return query;
        }
    }

}
