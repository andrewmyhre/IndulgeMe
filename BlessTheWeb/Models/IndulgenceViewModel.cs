﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlessTheWeb.Models
{
    public class IndulgenceViewModel
    {
        public int Id { get; set; }
        public string Confession { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string AmountDonated { get; set; }
        public string CharityName { get; set; }
    }
}