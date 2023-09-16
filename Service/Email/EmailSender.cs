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
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(_emailSettings.ApiBaseUri),
                Authenticator = new HttpBasicAuthenticator("api",_emailSettings.ApiKey),
            };

            RestRequest request = new RestRequest();
            request.AddParameter("domain", _emailSettings.Domain,ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("form", _emailSettings.From);
            request.AddParameter("to", emailMessage.To);
            request.AddParameter("subject", emailMessage.Subject);
            request.Method = Method.POST;

            TaskCompletionSource<IRestResponse> taskCompletionSource = new TaskCompletionSource<IRestResponse>();

            client.ExecuteAsync(request, r => taskCompletionSource.SetResult(r));

            RestResponse restResponse = (RestResponse)(await taskCompletionSource.Task);
        }

    }
}
