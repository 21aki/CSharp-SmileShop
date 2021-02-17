using System;

namespace SmileShop.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public Guid CreatedByUserID { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
    }
}
