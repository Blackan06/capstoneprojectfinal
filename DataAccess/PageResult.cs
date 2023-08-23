﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int RecordNumber { get; set; }
        public List<T> Items { get; set; }
    }
}
