using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileShop.Models
{
    [Table("User", Schema = "auth")]
    public class User : IId
    {
        public User()
        {
            Orders = new HashSet<Order>();
            ProductGroups = new HashSet<ProductGroup>();
            Products = new HashSet<Product>();
            UserRoles = new HashSet<UserRole>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ProductGroup> ProductGroups { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Inventory> Inventory_ { get; set; }

    }
}