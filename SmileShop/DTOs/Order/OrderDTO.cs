using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public int ItemCount { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
