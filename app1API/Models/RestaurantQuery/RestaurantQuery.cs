﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app1API.Models.RestaurantQuery
{
    public class RestaurantQuery
    {
        public string SearchPhrase { get; set; }
        public int  PageSize { get; set; }
        public int  PageNumber { get; set; }
        public string SortBy { get; set; }
        public SortDirection.SortDirection SortDirection { get; set; }

    }
}
