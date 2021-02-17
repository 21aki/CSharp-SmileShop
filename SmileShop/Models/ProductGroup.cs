using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileShop.Models
{
    public class ProductGroup
    {
        public ProductGroup()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        [Range(1, 999999)]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Reqired]
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }

        [Reqired]
        public User CreatedByUser { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
