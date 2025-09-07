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

        private string MerchantId => _config["Payment:MerchantId"] ?? throw new InvalidOperationException("MerchantId missing");
        private string SaltKey => _config["Payment:SaltKey"] ?? throw new InvalidOperationException("SaltKey missing");
        private string SaltIndex => _config["Payment:SaltIndex"] ?? "1";
        private string BaseUrl => _config["Payment:BaseUrl"] ?? throw new InvalidOperationException("BaseUrl missing");
        private string RedirectUrl => _config["Payment:RedirectUrl"] ?? "";
        private string CallbackUrl => _config["Payment:CallbackUrl"] ?? "";

        /// <summary>
        /// Creates a new order in PhonePe
        /// </summary>
        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createDto, string? idempotencyKey = null)
        {
            // 1. Build payload
            var payloadObj = new
            {
                merchantId = MerchantId,
                merchantTransactionId = createDto.OrderId,
                amount = (int)(createDto.Amount * 100), // convert ₹ to paise
                merchantUserId = string.IsNullOrEmpty(createDto.CustomerId) ? "guest" : createDto.CustomerId,
                redirectUrl = RedirectUrl,
                redirectMode = "POST",
                callbackUrl = CallbackUrl,
                paymentInstrument = new { type = "PAY_PAGE" }
            };

            string payloadJson = JsonSerializer.Serialize(payloadObj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            string base64Payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson));

            string apiEndpoint = "/pg/v1/pay";
            string requestUrl = $"{BaseUrl.TrimEnd('/')}{apiEndpoint}";

            // Signature
            string signature = GeneratePaySignature(base64Payload, apiEndpoint);

            // Request
            var requestBody = JsonSerializer.Serialize(new { request = base64Payload });

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("X-VERIFY", signature);
            request.Headers.Add("X-MERCHANT-ID", MerchantId);   // 🔑 Required
            

            if (!string.IsNullOrEmpty(idempotencyKey))
                request.Headers.Add("Idempotency-Key", idempotencyKey);

            // 4. Send
            var response = await _httpClient.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

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

        /// <summary>
        /// Check payment status
        /// </summary>
        public async Task<StatusResponseDto> CheckStatusAsync(string merchantTransactionId)
        {
            string apiEndpoint = $"/pg/v1/status/{MerchantId}/{merchantTransactionId}";
            string url = $"{BaseUrl.TrimEnd('/')}{apiEndpoint}";

            string signature = GenerateStatusSignature(apiEndpoint);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-VERIFY", signature);

            var response = await _httpClient.SendAsync(request);
            string body = await response.Content.ReadAsStringAsync();

            return new StatusResponseDto
            {
                Success = response.IsSuccessStatusCode,
                Code = response.IsSuccessStatusCode ? "PAYMENT_STATUS" : "HTTP_ERROR",
                Message = response.IsSuccessStatusCode ? "Payment status retrieved" : $"Error: {body}",
                Data = JsonSerializer.Deserialize<object>(body)
            };
        }

        /// <summary>
        /// Handle webhook callback
        /// </summary>
        public Task HandleWebhookAsync(WebhookPayloadDto payload, string signatureHeader)
        {
            string payloadJson = JsonSerializer.Serialize(payload);
            string expectedSignature = GenerateWebhookSignature(payloadJson);

            if (!string.Equals(expectedSignature, signatureHeader, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Invalid webhook signature");

            // TODO: update DB transaction status
            return Task.CompletedTask;
        }

        // -------------------- PRIVATE HELPERS --------------------

        // For /pg/v1/pay
        private string GeneratePaySignature(string base64Payload, string apiEndpoint)
        {
            string dataToHash = base64Payload + apiEndpoint + SaltKey;
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            var hashHex = Convert.ToHexString(hashBytes).ToLower();
            return $"{hashHex}###{SaltIndex}";
        }

        // For /pg/v1/status
        private string GenerateStatusSignature(string apiEndpoint)
        {
            string dataToHash = apiEndpoint + SaltKey;
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            var hashHex = Convert.ToHexString(hashBytes).ToLower();
            return $"{hashHex}###{SaltIndex}";
        }

        // For webhook verification
        private string GenerateWebhookSignature(string payloadJson)
        {
            string dataToHash = payloadJson + SaltKey;
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(dataToHash));
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}
