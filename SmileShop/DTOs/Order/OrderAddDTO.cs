using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class OrderAddDTO
    {

        public DateTime CreatedDate { get; set; }
        public decimal Discount { get; set; }

        public ICollection<OrderDetailDTO> OrderDetails { get; set; }
    }
}
