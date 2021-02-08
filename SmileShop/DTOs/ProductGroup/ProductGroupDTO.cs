using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.DTOs
{
    public class ProductGroupDTO
    {

        public int Id { get; set; }

        public string Name { get; set; }
        public UserDto CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }

    }
}
