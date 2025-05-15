using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class SmsService : ISmsService
{
    private readonly string accountSid = "AC57b0958a67db699373b89078628372b9";  // Remplace avec ton SID
    private readonly string authToken = "4292bee2e8313ae754d44190146abc97";  // Remplace avec ton Auth Token
    private readonly string fromNumber = "+15737993726"; // Numéro Twilio

    public SmsService()
    {
        TwilioClient.Init(accountSid, authToken);
    }

    public async Task<(string MessageSid, string Status)> SendSmsAsync(string toNumber, string message )
    {
       
        var messageStatus = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(fromNumber),
            to: new PhoneNumber(toNumber)
        );

        return (messageStatus.Sid, "SMS envoyé");
    }
}