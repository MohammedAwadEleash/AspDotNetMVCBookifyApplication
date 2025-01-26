using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Tasks
{
    public class HangfireTasks
    {



        private readonly IApplicationDbContext _context;
        private readonly IAppEnvironmentService _appEnvironmentService;

        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        private readonly IWhatsAppService _whatsAppService;

        public HangfireTasks(IApplicationDbContext context, IAppEnvironmentService appEnvironmentService, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender, IWhatsAppService whatsAppService)
        {
            _context = context;
            _whatsAppService = whatsAppService;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
            _appEnvironmentService = appEnvironmentService;
        }
        public async Task PrepareExpirationAlert()
        {




            var subscribers = _context.Subscribers.Include(s => s.Subscriptions)
                .Where(s => !s.Isblacklisted && s.Subscriptions.OrderByDescending(su => su.EndDate)
                .First().EndDate == DateTime.Today.AddDays(7)).ToList();

            foreach (var subscriber in subscribers)

            {
                var endDate = subscriber.Subscriptions.Last().EndDate.ToString("d, MMM, yyyy");

                var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]", "https://res.cloudinary.com/eleash/image/upload/v1734637220/resized_calendar_ufu2bg.png" },
                    { "[header]", $"Hello {subscriber.FirstName}" },
                    { "[body]", $"your subscription will be expired by {endDate}  🙁 \U0001F494" },
            };

                var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

                await _emailSender.SendEmailAsync(subscriber.Email, "your Bookify Subscription Expiration 📣 📩 ", body);

                //  BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(subscriber.Email, "Bookify Subscription Renewal", body), TimeSpan.FromMinutes(1));
                //  Send  WhatsApp message


                if (subscriber.HasWhatsApp)
                {


                    var mobileNumber = _appEnvironmentService.IsDevelopment ? "201001779462" : $"2{subscriber.MobileNumber}";

                    _whatsAppService.SendWhatsAppMessage(mobileNumber, $" your Bookify Subscription Expiration 📣 📩:\n Hello {subscriber.FirstName}, your subscription will be expired by {endDate} 🙁 💔");
                    //if (!string.IsNullOrEmpty(messageWhatsApp.ErrorMessage)) 
                    //{
                    //    return BadRequest(messageWhatsApp.ErrorMessage);

                    //}
                }


            }

        }





        public async Task RentalExpirationAlert()
        {


            var tomorrow = DateTime.Today.AddDays(1);

            var rentals = _context.Rentals.Include(r => r.subscriber)

              .Include(r => r.RentalCopies).ThenInclude(rc => rc.BookCopy).ThenInclude(c => c!.Book)
                 .Where(r => r.RentalCopies.Any(rc => rc.EndDate.Date == tomorrow && !rc.ReturnDate.HasValue)).ToList();

            //var rental = _context.RentalCopies.Include(rc => rc.Rental).ThenInclude(r => r.subscriber)
            //    .Where(r => r.EndDate == DateTime.Today.AddDays(1));


            foreach (var rental in rentals)

            {
                var expiredCopies = rental.RentalCopies.Where(rc => rc.EndDate.Date == tomorrow && !rc.ReturnDate.HasValue);

                var message = $"your rental for the below book(s) will be expired by tomorrow {tomorrow.ToString("dd MMM, yyyy")} 💔:";
                var messageW = message;

                message += "<ol type='I'>";
                int i = 0;
                foreach (var copy in expiredCopies)
                {
                    i++;
                    message += $"<li>{copy.BookCopy!.Book!.Title}</li>";
                    messageW += $" \n {i}-{copy.BookCopy!.Book!.Title}";

                }
                message += "</ol>";


                var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]", "https://res.cloudinary.com/eleash/image/upload/v1734637220/resized_calendar_ufu2bg.png" },
                    { "[header]", $"Hello {rental.subscriber!.FirstName} 💖," },
                    { "[body]", message },
            };

                var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

                await _emailSender.SendEmailAsync(rental.subscriber!.Email, "Bookify Rental Expiration 🔔", body);

                //  BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(subscriber.Email, "Bookify Subscription Renewal", body), TimeSpan.FromMinutes(1));
                //  Send  WhatsApp message


                if (rental.subscriber.HasWhatsApp)
                {


                    var mobileNumber = _appEnvironmentService.IsDevelopment ? "201001779462" : $"2{rental.subscriber.MobileNumber}";

                    _whatsAppService.SendWhatsAppMessage(mobileNumber, $"Bookify Rental Expiration 🔔: \n Hello {rental.subscriber.FirstName} 💖, \n  {messageW}");

                }

            }

        }


    }
}
