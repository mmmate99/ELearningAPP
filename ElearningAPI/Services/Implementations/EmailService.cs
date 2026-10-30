using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                
                var smtpClient = new SmtpClient(smtpSettings["Host"])
                {
                    Port = int.Parse(smtpSettings["Port"]!),
                    Credentials = new NetworkCredential(
                        smtpSettings["Username"], 
                        smtpSettings["Password"]),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"]!)
                };

                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {toEmail}: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {toEmail}");
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Üdvözöljük az E-Learning Platformon!";
            var body = $@"
                <html>
                <body>
                    <h2>Kedves {userName}!</h2>
                    <p>Köszönjük, hogy regisztrált az E-Learning Platformra!</p>
                    <p>Most már hozzáférhet sokféle tanfolyamhoz, leckéhez és kvízhez.</p>
                    <p>Kellemes tanulást kívánunk!</p>
                    <br>
                    <p>Üdvözlettel,<br>E-Learning Csapat</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendCourseEnrollmentEmailAsync(string toEmail, string userName, string courseTitle)
        {
            var subject = $"Sikeresen feliratkozott a(z) {courseTitle} kurzusra";
            var body = $@"
                <html>
                <body>
                    <h2>Kedves {userName}!</h2>
                    <p>Sikeresen feliratkozott a(z) <strong>{courseTitle}</strong> kurzusra.</p>
                    <p>Most már elkezdheti a tanulást. A kurzus anyagai a profilja alatt érhetők el.</p>
                    <p>Jó tanulást kívánunk!</p>
                    <br>
                    <p>Üdvözlettel,<br>E-Learning Csapat</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            var resetLink = $"{_configuration["ClientUrl"]}/reset-password?token={resetToken}";
            
            var subject = "Jelszó visszaállítás";
            var body = $@"
                <html>
                <body>
                    <h2>Jelszó visszaállítás</h2>
                    <p>Kérjük, kattintson az alábbi linkre a jelszó visszaállításához:</p>
                    <p><a href='{resetLink}'>Jelszó visszaállítása</a></p>
                    <p>Ha nem Ön kérte a jelszó visszaállítást, kérjük, hagyja figyelmen kívül ezt az emailt.</p>
                    <br>
                    <p>Üdvözlettel,<br>E-Learning Csapat</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendQuizResultEmailAsync(string toEmail, string userName, string quizTitle, bool passed, int score)
        {
            var result = passed ? "sikeres" : "sikertelen";
            
            var subject = $"{quizTitle} kvíz eredménye";
            var body = $@"
                <html>
                <body>
                    <h2>Kedves {userName}!</h2>
                    <p>A(z) <strong>{quizTitle}</strong> kvíz kitöltése <strong>{result}</strong> volt.</p>
                    <p>Elért pontszám: <strong>{score}</strong></p>
                    <p>{GetResultMessage(passed, score)}</p>
                    <br>
                    <p>Üdvözlettel,<br>E-Learning Csapat</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendForumNotificationEmailAsync(string toEmail, string userName, string postTitle, string authorName)
        {
            var subject = $"Új válasz a fórumon: {postTitle}";
            var body = $@"
                <html>
                <body>
                    <h2>Kedves {userName}!</h2>
                    <p><strong>{authorName}</strong> válaszolt a(z) <strong>{postTitle}</strong> témában.</p>
                    <p>Látogasson el a fórumra, hogy megtekintse a választ.</p>
                    <br>
                    <p>Üdvözlettel,<br>E-Learning Csapat</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private string GetResultMessage(bool passed, int score)
        {
            if (passed)
            {
                return score >= 90 
                    ? "Kiváló teljesítmény! Gratulálunk!"
                    : score >= 70 
                        ? "Jó teljesítmény! Még egy kicsit gyakorolj, hogy tökéletes legyél!"
                        : "Átmentél, de érdemes lenne még gyakorolni.";
            }
            else
            {
                return "Ne csüggedj! Próbáld újra, biztosan sikerülni fog!";
            }
        }
    }
}