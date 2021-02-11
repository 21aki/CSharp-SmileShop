using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmileShop.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public int Debit { get; set; }

        public int Credit { get; set; }

        //public decimal Price { get; set; }

        public string Remark { get; set; }

        public Guid CreatedByUserId { get; set; }

        public virtual Product Product_ { get; set; }

        public virtual User CreatedByUser { get; set; }
    }
}
