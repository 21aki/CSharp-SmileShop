namespace SmileShop.DTOs
{
    public class OrderDetailProcessDTO
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal DiscountPrice { get; set; }

    }
}
