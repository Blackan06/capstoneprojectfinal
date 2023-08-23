using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.PrizeDto
{
    public class GetPrizeDto : BasePrizeDto, IBaseDto
    {
        public Guid Id { get; set; }
        public string RankNumber { get; set; }
    }
}
