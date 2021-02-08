﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Models
{
    public class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int GroupId { get; set; }

        [Reqired]
        [MaxLength(50)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? Status { get; set; }

        public virtual ProductGroup Group_ { get; set; }
        public virtual User CreatedByUser_ { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}