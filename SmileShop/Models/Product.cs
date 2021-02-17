using System;
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
            Stock = new HashSet<Stock>();
        }

        [Key]
        [Range(1,999999)]
        public int Id { get; set; }

        [Reqired]
        [Range(1, 999999)]
        public int GroupId { get; set; }

        [Reqired]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Reqired]
        public decimal Price { get; set; }

        [Reqired]
        public int StockCount { get; set; }

        [Reqired]
        public Guid? CreatedByUserId { get; set; }

        [Reqired]
        public DateTime CreatedDate { get; set; }
        
        [Reqired]
        public bool? Status { get; set; }

        public virtual ProductGroup Group { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }

    }
}
