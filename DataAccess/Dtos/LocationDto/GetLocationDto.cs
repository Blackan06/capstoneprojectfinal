﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos.LocationDto
{
    public class GetLocationDto : BaseLocationDto , IBaseDto
    {
        public Guid Id { get; set; }
    }
}
