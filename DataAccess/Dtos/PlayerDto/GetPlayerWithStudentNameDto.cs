using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerDto
{
    public class GetPlayerWithStudentNameDto
    {
        public string SchoolName { get; set; }
        public string StudentName { get; set; }
        public string EventName { get; set; }
        public string Passcode { get; set; }

    }
}
