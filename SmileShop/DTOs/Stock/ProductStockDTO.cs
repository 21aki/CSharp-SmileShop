using System;

namespace SmileShop.DTOs
{
    public class ProductStockDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int ProductGroupId { get; set; }

        public String ProductGroupName { get; set; }

        public int Debit { get; set; }

        public int Credit { get; set; }

        public int StockBefore { get; set; }

        public int StockAfter { get; set; }


        public string Remark { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public String CreatedByUserName { get; set; }

    }
}
