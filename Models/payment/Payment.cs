namespace APiLoginWebApplication.Models.payment
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public string OrderId { get; set; }
        public string MerchantTransactionId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
