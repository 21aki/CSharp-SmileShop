﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }
        public Guid CreatedByUserId { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
