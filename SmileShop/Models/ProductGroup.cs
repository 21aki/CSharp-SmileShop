using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Models
{
    public class ProductGroup
    {
        public ProductGroup()
        {
            Products = new HashSet<Product>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public Guid CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Status { get; set; }

        public User CreatedUser_ { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
