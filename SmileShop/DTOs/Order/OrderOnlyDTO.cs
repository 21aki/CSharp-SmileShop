using System;

namespace SmileShop.DTOs
{
    public class OrderOnlyDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
        public decimal Net { get; set; }

        public UserDto CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
