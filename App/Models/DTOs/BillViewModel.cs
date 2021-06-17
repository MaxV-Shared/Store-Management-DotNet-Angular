﻿using App.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models.DTOs
{
    public class BillViewModel
    {
        public string CustomerPhoneNumber { get; set; }
        public int UserPaymentId { get; set; }
        public int UserPaymentUserName { get; set; }
        public double? TotalPrice { get; set; }
        public double? DiscountPrice { get; set; }
    }
}
