using System;
using System.Collections.Generic;

namespace SmileShop.DTOs
{
    public class OrderAddDTO
    {

        public DateTime CreatedDate { get; set; }
        public decimal Discount { get; set; }

        public ICollection<OrderDetailAddDTO> OrderDetails { get; set; }
    }
}
