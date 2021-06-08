﻿using App.Models.DTOs;
using App.Models.Entities;
using App.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Interface
{
    public interface  IBillService : IBaseService<Bill, BillCreateRequest, BillViewModel, int>
    { 
        public Task<DiscountViewModel> PostAsync(BillCreateRequest request);
    }
}
