using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class SmsService : ISmsService
{

// TWILIO INFO A METTRE ICI
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