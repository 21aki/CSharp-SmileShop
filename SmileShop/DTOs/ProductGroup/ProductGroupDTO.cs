using System;

namespace SmileShop.DTOs
{
    public class ProductGroupDTO
    {

        public int Id { get; set; }

        public string Name { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }

    }
}
