namespace APiLoginWebApplication.Models.payment
{
    public class Webhook
    {
        public long WebhookId { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
