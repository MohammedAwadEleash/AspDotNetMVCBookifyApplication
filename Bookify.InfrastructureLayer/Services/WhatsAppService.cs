using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Bookify.Web.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly TwilioSettings _twilioSettings;

        public WhatsAppService(IOptions<TwilioSettings> twilioSettings)
        {


            _twilioSettings = twilioSettings.Value;
        }

        public MessageResource SendWhatsAppMessage(string toWhatsAppNumber, string messageBody)
        {
            TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);

            var message = MessageResource.Create(
                body: messageBody,
                from: new PhoneNumber($"whatsapp:{_twilioSettings.TwilioPhoneNumber}"),
                to: new PhoneNumber($"whatsapp:{toWhatsAppNumber}")
            );

            return message;


        }
    }
}