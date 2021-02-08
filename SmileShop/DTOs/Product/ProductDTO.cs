using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public ProductDTOProductGroup Group { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public UserDto CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }
    }
}
