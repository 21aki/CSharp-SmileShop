using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class ProductFilterDTO
    {
        public int? Id { get; set; }
        public int? GroupId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public Guid? CreatedBy { get; set; }
        public bool? Status { get; set; }

    }
}
