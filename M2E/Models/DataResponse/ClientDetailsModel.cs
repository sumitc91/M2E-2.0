﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M2E.Models.DataResponse
{
    public class ClientDetailsModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string RequestUrlAuthority { get; set; }
        public string imageUrl { get; set; }
        public string gender { get; set; }
        public string isLocked { get; set; }
        public string totalReputation { get; set; }
        public string gold { get; set; }
        public string silver { get; set; }
        public string bronze { get; set; }
    }
}