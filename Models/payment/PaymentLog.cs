namespace APiLoginWebApplication.Models.payment
{
    public class PaymentLog
    {
        public long LogId { get; set; }
        public Guid PaymentId { get; set; }
        public string RequestPayload { get; set; }
        public string ResponsePayload { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
