using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using RestSharp;
using RestSharp.Authenticators;

namespace Service.Email
{
    public class EmailSender : IEmailSender
    {
        public EmailSettings _emailSettings { get; }
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Host = _emailSettings.SmtpServer;
                    client.Port = _emailSettings.Port;
                    client.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);
                    client.EnableSsl = true; // Đảm bảo sử dụng SSL khi gửi email (nếu cần)

                    var message = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.UserName),
                        Subject = emailMessage.Subject,
                        Body = emailMessage.Body,
                        IsBodyHtml = true
                    };

                    message.To.Add(emailMessage.To);

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ hoặc ghi log tùy theo yêu cầu
                throw ex;
            }
        }

    }
}
