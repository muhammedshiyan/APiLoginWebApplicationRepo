
using APiLoginWebApplication.Models;

namespace APiLoginWebApplication.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createDto, string idempotencyKey = null);
        Task<StatusResponseDto> CheckStatusAsync(string merchantTransactionId);
        Task HandleWebhookAsync(WebhookPayloadDto payload, string signatureHeader);
    }
}
