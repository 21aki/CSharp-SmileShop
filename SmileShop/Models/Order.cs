using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public int ItemCount { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public decimal Discount { get; set; }

        [Required]
        public decimal Net { get; set; }

        [Required]
        public Guid CreatedByUserId { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
