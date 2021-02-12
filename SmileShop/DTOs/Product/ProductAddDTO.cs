using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class ProductAddDTO
    {
        public int GroupId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
