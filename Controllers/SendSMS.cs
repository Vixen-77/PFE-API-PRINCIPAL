using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using DotNetEnv; 


namespace SmsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
    private readonly string accountSid = "AC57b0958a67db699373b89078628372b9";  // Remplace avec ton SID
    private readonly string authToken = "4292bee2e8313ae754d44190146abc97";  // Remplace avec ton Auth Token
    private readonly string fromNumber = "+15737993726"; // Numéro Twilio


        public SmsController()
        {
            // Initialisation de Twilio avec les identifiants du compte
            TwilioClient.Init(accountSid, authToken);
        }

        /// <summary>
        /// Envoie un SMS avec un lien Google Maps
        /// </summary>
        /// <param name="">Numéro du destinataire</param>
        /// <param name="">Message à envoyer (optionnel)</param>
        /// <returns>Statut du message envoyé</returns>
        /// 

        [HttpPost("send-sms")]
        public IActionResult SendSms([FromQuery] string toNumber, [FromQuery] string message )
        {
            try
            {
                // Coordonnées GPS
                double latitude = 36.75;  // Remplace par ta latitude
                double longitude = 3.06;  // Remplace par ta longitude
                
                // Crée un lien Google Maps avec les coordonnées
                string mapsLink = $"https://www.google.com/maps?q={latitude},{longitude}";

                // Si un message n'est pas fourni, envoie par défaut un message avec la position GPS
                if (string.IsNullOrEmpty(message))
                {
                    message = $"Voici ma position GPS : {mapsLink}";
                }
                else
                {
                    message += $" - Ma position GPS : {mapsLink}";
                }

                // Envoi du SMS via Twilio
                var messageStatus = MessageResource.Create(
                    body: message,
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(toNumber)
                );

                return Ok(new { MessageSid = messageStatus.Sid, Status = "SMS envoyé" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Erreur lors de l'envoi du SMS : {ex.Message}" });
            }
        }
    }
}