namespace Bookify.Application.Comman.Interfaces
{
    public interface IEmailBodyBuilder
    {

        string GetEmailBody(string template, Dictionary<string, string> placeholders);
    }
}
