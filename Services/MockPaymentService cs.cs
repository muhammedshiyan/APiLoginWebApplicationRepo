using APiLoginWebApplication.Models;
using APiLoginWebApplication.Services.Interfaces;

namespace APiLoginWebApplication.Services
{
    public class MockPaymentService : IPaymentService
    {
        public Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createDto, string idempotencyKey = null)
        {
            return Task.FromResult(new OrderResponseDto
            {
                Success = true,
                Code = "MOCK_ORDER",
                Message = "Mock order created",
                //Data = new { redirectUrl = "https://example.com/mock-payment" }
                Data = new { redirectUrl = "http://localhost:4200/payment/success" }
            });
        }

        public Task<StatusResponseDto> CheckStatusAsync(string merchantTransactionId)
        {
            return Task.FromResult(new StatusResponseDto
            {
                Success = true,
                Code = "MOCK_STATUS",
                Message = "Mock payment success",
                Data = new { status = "SUCCESS" }
            });
        }

        public Task HandleWebhookAsync(WebhookPayloadDto payload, string signatureHeader)
        {
            return Task.CompletedTask;
        }
    }
}
