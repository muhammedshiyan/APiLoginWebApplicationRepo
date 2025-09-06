namespace APiLoginWebApplication.Models
{
    public class WebhookPayloadDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Signature { get; set; } = string.Empty;
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
