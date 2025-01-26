using Twilio.Rest.Api.V2010.Account;

namespace Bookify.Application.Comman.Interfaces
{

    public interface IWhatsAppService
    {
        MessageResource SendWhatsAppMessage(string toWhatsAppNumber, string messageBody);
    }
}

