namespace APiLoginWebApplication.Models
{
    public class CreateOrderDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string CustomerId { get; set; } = string.Empty;
    }
}
