using DataAccess.Dtos.PlayerPrizeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage emailMessage);

    }
}
