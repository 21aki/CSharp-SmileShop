﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class OrderDetailAddDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}