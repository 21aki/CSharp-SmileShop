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

        [Required]
        public int ProductId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int Debit { get; set; }

        [Required]
        public int Credit { get; set; }

        public int StockBefore { get; set; }

        public virtual int StockAfter {
            get {
                return this.StockBefore + Debit - Credit;
            }
        }
        

        //public decimal Price { get; set; }

        public string Remark { get; set; }

        [Required]
        public Guid? CreatedByUserId { get; set; }

        public virtual Product Product { get; set; }

        public virtual User CreatedByUser { get; set; }

    }
}
