using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Email
{
    public class EmailSettings
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }
        public string ApiBaseUri { get; set; }
        public string FromName { get; set; }
        public string From { get; set; }
        public string AdminMail { get; set; }
    }
}
