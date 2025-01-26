namespace Bookify.Application.Services
{
    internal class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriberService _subscriberService;


        public RentalService(IUnitOfWork unitOfWork, ISubscriberService subscriberService)
        {
            _unitOfWork = unitOfWork;
            _subscriberService = subscriberService;
        }

        public IQueryable<Rental?> GetQueryableDetails()
        {
            //// this function get  reference to  all rentals with RentalCopies, BookCopy, Book (but they still in database)
            var query = _unitOfWork.Rentals.GetQueryable().AsNoTracking().Include(r => r.RentalCopies).ThenInclude(rc => rc.BookCopy!).ThenInclude(c => c.Book);

            return query;
        }
        public Rental? RentalDetails(int id)
        {

            var rental = _unitOfWork.Rentals.Find(r => r.Id == id, r => r.Include(r => r.RentalCopies).ThenInclude(rc => rc.BookCopy!).ThenInclude(c => c.Book!));

            if (rental is null)
                return null;
            return rental;
        }



        public string ValidateExtendedCopies(Subscriber subscriber, Rental rental)
        {

            var errorMessage = string.Empty;
            if (subscriber!.Isblacklisted)
                errorMessage = Errors.RentalNotAllowedForBlacklisted;

            else if (subscriber!.Subscriptions.Last().EndDate < rental.StartDate.AddDays((int)RentalsConfiguration.MaxRentalDuration))
                errorMessage = Errors.RentalNotAllowedForInactive;


            else if (rental.StartDate.AddDays((int)RentalsConfiguration.RentalDuration) < DateTime.Today)
                errorMessage = Errors.ExtendNotAllowed;

            return errorMessage;
        }


        public bool ISAllowExtend(Subscriber subscriber, Rental rental)
        {
            var AllowOrNot = !subscriber!.Isblacklisted &&
                subscriber.Subscriptions.Last().EndDate >= rental.StartDate.AddDays((int)RentalsConfiguration.MaxRentalDuration)
                && rental.StartDate.AddDays((int)RentalsConfiguration.RentalDuration) >= DateTime.Today;
            return AllowOrNot;
        }
        public bool IsCopyInRental(int copyId)
        {

            return _unitOfWork.RentalCopies.IsExists(rc => rc.BookCopyId == copyId && !rc.ReturnDate.HasValue);
        }


        public void Return(Rental rental, IEnumerable<ReturnCopyDto> copies, bool penaltyPaid, string updatedById)
        {
            var isUpdated = false;
            // Return Copy:
            foreach (var copy in copies)

            {
                ///Case :  If the copy is never returned and extended (He didn't click on any of them)
                if (!copy.IsReturned.HasValue)
                    continue;
                var currentCopy = rental.RentalCopies.SingleOrDefault(rc => rc.BookCopyId == copy.Id);
                if (currentCopy is null)
                    continue;

                // /Case:if he click on Return Copy button 
                if (copy.IsReturned.HasValue && copy.IsReturned.Value)
                {
                    if (currentCopy.ReturnDate.HasValue)
                        continue;


                    currentCopy.ReturnDate = DateTime.Now;

                    isUpdated = true;


                }

                // Case:if he click on Extend Copy button 

                if (copy.IsReturned.HasValue && !copy.IsReturned.Value)
                {

                    if (currentCopy.ExtendedOn.HasValue)
                        continue;

                    currentCopy.ExtendedOn = DateTime.Now;
                    currentCopy.EndDate = currentCopy.RentalDate.AddDays((int)RentalsConfiguration.MaxRentalDuration);
                    isUpdated = true;
                }



            }

            if (isUpdated)
            {
                rental.LastUpdatedOn = DateTime.Now;
                rental.LastUpdatedById = updatedById;
                rental.PenaltyPaid = penaltyPaid;
                _unitOfWork.Complete();
            };
        }


        public BookCopy? GetCopyBySerialNumber(string serialNumber)
        {

            var copy = _unitOfWork.BookCopies
                    .Find(predicate: c => c.SerialNumber.ToString() == serialNumber && !c.IsDeleted && !c.Book!.IsDeleted,
                          include: c => c.Include(x => x.Book)!);


            if (copy is null)
                return null;
            return copy;

        }

        public int GetNumberOfCopies(int rentalId)
        {
            return _unitOfWork.RentalCopies.Count(rc => rc.RentalId == rentalId);
        }


        public (string errorMessage, int? maxAllowedCopies) ValidateSubscriber(int susbscriberId, int? rentalId = null)
        {


            var subscriber = _subscriberService.GetQueryableSubscribersWithRentals().SingleOrDefault(s => s.Id == susbscriberId);

            if (subscriber is null)
                return (errorMessage: Errors.NotFoundSubscriber, maxAllowedCopies: null);


            if (subscriber.Isblacklisted)

                return (errorMessage: Errors.BlackListedSubscriber, maxAllowedCopies: null);




            if (subscriber.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalsConfiguration.RentalDuration))
                return (errorMessage: Errors.InactiveSubscriber, maxAllowedCopies: null);


            var currentRentals = subscriber.Rentals.Where(r => rentalId == null || r.Id != rentalId).SelectMany(r => r.RentalCopies).Count(rc => !rc.ReturnDate.HasValue);

            // If ReturnDate is null, the copy has not been returned yet          

            var availableCopiesCount = ((int)RentalsConfiguration.MaxAllowedCopies) - currentRentals;


            if (availableCopiesCount <= 0)
                return (errorMessage: Errors.MaxCopiesReached, maxAllowedCopies: null);

            return (errorMessage: "", maxAllowedCopies: availableCopiesCount);


        }
        public (string errorMessage, ICollection<RentalCopy> copies) ValidateCopies(IList<int> selectedCopiesBeforeFilter, int subscriberId, int? rentalId = null)
        {


            List<RentalCopy> copies = new List<RentalCopy>();


            var subscriber = _subscriberService.GetQueryableSubscribersWithRentals().SingleOrDefault(s => s.Id == subscriberId);
            var currentRentals = subscriber!.Rentals.Where(r => rentalId == null || r.Id != rentalId).SelectMany(r => r.RentalCopies).Count(rc => !rc.ReturnDate.HasValue);

            var currentRentalCopies = selectedCopiesBeforeFilter.Count() + currentRentals;

            var availableCopiesCount = ((int)RentalsConfiguration.MaxAllowedCopies) - currentRentalCopies;


            if (availableCopiesCount < 0)
                return (errorMessage: Errors.ExceedTheMax, copies);






            var selectedCopiesAfterFilter = _unitOfWork.BookCopies.FindAll(predicate: c => selectedCopiesBeforeFilter.Contains(c.SerialNumber)

                , include: c => c.Include(c => c.Book).Include(c => c.Rentals)).ToList();


            // Here : I get BookId for copy  that rented now
            var currentSubscriberBooksRented = _unitOfWork.Rentals.FindAll(r => r.SubscriberId == subscriberId && (rentalId == null || r.Id != rentalId),
                include: r => r.Include(r => r.RentalCopies!).ThenInclude(rc => rc.BookCopy!))
              .SelectMany(r => r.RentalCopies)
              .Where(rc => !rc.ReturnDate.HasValue)
               .Select(rc => rc.BookCopy!.BookId)
              .ToList();




            foreach (var copy in selectedCopiesAfterFilter)
            {
                if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)

                    return (errorMessage: Errors.NotAvilableRental, copies);

                // after you retrive the copies from DB you need check wether copy is rented or not :

                if (copy.Rentals.Any(rc => !rc.ReturnDate.HasValue && (rentalId == null || rc.RentalId != rentalId)))
                    return (errorMessage: Errors.CopyIsInRental, copies);

                if (currentSubscriberBooksRented.Any(bookId => bookId == copy.BookId))
                    return (errorMessage: $"This subscriber already has a copy for '{copy.Book.Title}' Book", copies);

                copies.Add(new RentalCopy { BookCopyId = copy.Id });




            }

            return (errorMessage: "", copies);

        }

        public Rental Create(int subscriberId, ICollection<RentalCopy> copies, string createdById)
        {

            var rental = new Rental()
            {
                SubscriberId = subscriberId,
                RentalCopies = copies,
                CreatedById = createdById
            };

            _unitOfWork.Rentals.Add(rental);
            _unitOfWork.Complete();

            return rental;
        }




        public Rental Update(int rentalId, ICollection<RentalCopy> copies, string updatedById)
        {




            var rental = _unitOfWork.Rentals.GetById(rentalId);

            rental!.RentalCopies = copies!;
            rental.LastUpdatedById = updatedById;
            rental.LastUpdatedOn = DateTime.Now;

            _unitOfWork.Complete();
            return rental;
        }

        public IEnumerable<BookCopy> GetRentalCopies(IEnumerable<int> copies)
        {


            var currentCopies = _unitOfWork.BookCopies.FindAll(predicate: c => copies.Contains(c.Id),
               include: c => c.Include(c => c.Book!));
            return currentCopies;
        }

        public Rental? MarkAsDeleted(int rentalId, string updatedById)
        {

            var rental = _unitOfWork.Rentals.GetById(rentalId);

            if (rental is null || rental.CreatedOn.Date != DateTime.Today)
                return null;



            rental.IsDeleted = true;
            rental.LastUpdatedOn = DateTime.Now;
            rental.LastUpdatedById = updatedById;

            _unitOfWork.Complete();

            return rental;
        }
        //Dashboard:
        public IEnumerable<KeyValuePairDto> GetRentalsPerDay(DateTime? startDate, DateTime? endDate)
        {

            var data = _unitOfWork.RentalCopies.GetQueryable()
                .Where(rc => rc.RentalDate >= startDate && rc.RentalDate <= endDate)
                .GroupBy(rc => new
                {
                    Date = rc.RentalDate
                }).Select(g => new KeyValuePairDto
                (
                   g.Key.Date.ToString("d MMM"),
                    g.Count().ToString()

                )
                ).ToList();
            return data;



            //var figures = new List<ChartItemViewModel>();
            //for (var day = startDate; day <= endDate; day = day.Value.AddDays(1))

            //{


            //    var dataDay = data.SingleOrDefault(d => d.Label == day.Value.ToString("d MMM"));

            //    ChartItemViewModel item = new()
            //    {


            //        Label = day.Value.ToString("d MMM"),

            //        Value = dataDay is null ? "0" : dataDay.Value

            //    }; 
            //    figures.Add(item);
            //}
        }

        public IEnumerable<KeyValuePairDto> GetDelayedBooks(int year)
        {
            var delayedBooks = _unitOfWork.RentalCopies.GetQueryable()
                           .Where(rc => rc.EndDate.Year == year && ((rc.ReturnDate.HasValue && rc.ReturnDate > rc.EndDate)
                           || (!rc.ReturnDate.HasValue && DateTime.Today > rc.EndDate)))
                           .GroupBy(rc => new
                           {
                               Month = rc.EndDate.Month

                           }).Select(g => new KeyValuePairDto
                           (

                                new DateTime(year, g.Key.Month, 1).ToString("MMMM"),
                                g.Count().ToString()
                           )).ToList();

            return delayedBooks;
        }

        public IEnumerable<KeyValuePairDto> GetUnDelayedBooks(int year)
        {
            var unDelayedBooks = _unitOfWork.RentalCopies.GetQueryable()
                           .Where(rc => rc.EndDate.Year == year && ((rc.ReturnDate.HasValue && rc.ReturnDate <= rc.EndDate)
                           || (!rc.ReturnDate.HasValue && DateTime.Today <= rc.EndDate)))
                           .GroupBy(rc => new
                           {
                               Month = rc.EndDate.Month

                           }).Select(g => new KeyValuePairDto
                           (

                                new DateTime(year, g.Key.Month, 1).ToString("MMMM"),
                                g.Count().ToString()
                           )).ToList();

            return unDelayedBooks;
        }

        public IEnumerable<KeyValuePairDto> GetExtendedBooks(int year)
        {
            var extendedBooks = _unitOfWork.RentalCopies.GetQueryable()
                           .Where(rc => rc.ExtendedOn.HasValue && rc.ExtendedOn.Value.Year == year)
                           .GroupBy(rc => new
                           {
                               Month = rc.EndDate.Month

                           }).Select(g => new KeyValuePairDto
                           (

                                new DateTime(year, g.Key.Month, 1).ToString("MMMM"),
                                g.Count().ToString()
                           )).ToList();

            return extendedBooks;
        }
    }



}
