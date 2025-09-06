using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using APiLoginWebApplication.Models;
using APiLoginWebApplication.Services.Interfaces;

namespace APiLoginWebApplication.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaymentService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private string MerchantId => _config["Payment:MerchantId"]!;
        private string SaltKey => _config["Payment:SaltKey"]!;
        private string BaseUrl => _config["Payment:BaseUrl"]!; // sandbox or production

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createDto, string idempotencyKey = null)
        {
            var payload = new
            {
                merchantId = MerchantId,
                transactionId = createDto.OrderId,
                amount = createDto.Amount,
                currency = createDto.Currency,
                customerId = createDto.CustomerId,
                redirectUrl = _config["Payment:RedirectUrl"]
            };

            string json = JsonSerializer.Serialize(payload);
            string signature = GenerateSignature(json);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/orders");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Headers.Add("X-VERIFY", signature);

            if (!string.IsNullOrEmpty(idempotencyKey))
                request.Headers.Add("Idempotency-Key", idempotencyKey);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new OrderResponseDto
                {
                    Success = false,
                    Code = "HTTP_ERROR",
                    Message = $"Error: {body}"
                };
            }

            return new OrderResponseDto
            {
                Success = true,
                Code = "ORDER_CREATED",
                Message = "Order created successfully",
                Data = JsonSerializer.Deserialize<object>(body)
            };
        }

        public async Task<StatusResponseDto> CheckStatusAsync(string merchantTransactionId)
        {
            string url = $"{BaseUrl}/status/{MerchantId}/{merchantTransactionId}";
            string signature = GenerateSignature(merchantTransactionId);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-VERIFY", signature);

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            return new StatusResponseDto
            {
                Success = response.IsSuccessStatusCode,
                Code = response.IsSuccessStatusCode ? "PAYMENT_STATUS" : "HTTP_ERROR",
                Message = response.IsSuccessStatusCode ? "Payment status retrieved" : $"Error: {body}",
                Data = JsonSerializer.Deserialize<object>(body)
            };
        }

        public Task HandleWebhookAsync(WebhookPayloadDto payload, string signatureHeader)
        {
            // 1. Verify signature
            string expectedSignature = GenerateSignature(JsonSerializer.Serialize(payload));
            if (expectedSignature != signatureHeader)
                throw new UnauthorizedAccessException("Invalid webhook signature");

            // 2. Save/update DB record for transaction
            // Example: _db.Payments.UpdateStatus(payload.TransactionId, payload.Status);

            return Task.CompletedTask;
        }

        private string GenerateSignature(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SaltKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
