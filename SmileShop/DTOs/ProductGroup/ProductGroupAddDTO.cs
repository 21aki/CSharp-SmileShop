using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class ProductGroupAddDTO
    {
        public string Name { get; set; }

        public bool Status { get; set; }
    }
}
