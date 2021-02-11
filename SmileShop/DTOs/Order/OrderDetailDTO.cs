using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class OrderDetailDTO
    {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
    }
}
