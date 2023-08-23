using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PlayerPrizeDto
{
    public class GetPlayerPrizeDto : BasePlayerPrizeDto , IBaseDto
    {
        public Guid Id { get; set; }
    }
}
