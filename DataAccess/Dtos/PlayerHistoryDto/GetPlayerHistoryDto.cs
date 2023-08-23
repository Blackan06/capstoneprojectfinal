using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerHistoryDto
{
    public class GetPlayerHistoryDto : BasePlayerHistoryDto , IBaseDto
    {
        public Guid Id { get; set; }
    }
}
