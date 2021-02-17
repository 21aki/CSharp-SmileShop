using System;
using System.ComponentModel.DataAnnotations;

namespace SmileShop.Models
{
    public class OrderDetail
    {
        [Reqired]
        [Range(0, 999999)]
        public int OrderId { get; set; }
        [Reqired]
        [Range(0, 999999)]
        public int ProductId { get; set; }
        [Reqired]
        [Range(0, 999999)]
        public int Quantity { get; set; }
        //[Reqired]
        public decimal Price { get; set; }
        //[Reqired]
        public decimal DiscountPrice { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
