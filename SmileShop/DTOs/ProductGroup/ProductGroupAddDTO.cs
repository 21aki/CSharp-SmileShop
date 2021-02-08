using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class ProductGroupAddDTO
    {

        [Reqired]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
