/*using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;


public class PasswordResetHub : Hub
{
    private static Dictionary<string, string> codes = new();

    public async Task RequestCode(string email, string oldPassword)
    {
        // Vérifie le mot de passe
        if (!GlobalService.VerifyPassword(email, oldPassword)) {
            await Clients.Caller.SendAsync("Error", "Mot de passe incorrect");
            return;
        }

        // Génère un code et envoie
        var code = VerificationCodeService.GenerateCode(email);
        EmailService.SendCode(email, code);

        // Retourne succès
        await Clients.Caller.SendAsync("CodeSent", "Un code a été envoyé à votre email");
    }

    public async Task SubmitCode(string email, string code)
    {
        if (VerificationCodeService.ValidateCode(email, code)) {
            await Clients.Caller.SendAsync("CodeValid", "Code correct, tu peux changer ton mot de passe");
        } else {
            await Clients.Caller.SendAsync("Error", "Code invalide");
        }
    }
}
*/