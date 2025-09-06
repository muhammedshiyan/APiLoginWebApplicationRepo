using APiLoginWebApplication.Models;
using APiLoginWebApplication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace APiLoginWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model)
        {
            if (model.Amount <= 0)
                return BadRequest("Invalid amount");

            var result = await _paymentService.CreateOrderAsync(model);
            return Ok(result);
        }

        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetStatus(string transactionId)
        {
            var result = await _paymentService.CheckStatusAsync(transactionId);
            return Ok(result);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback(
            [FromBody] WebhookPayloadDto payload,
            [FromHeader(Name = "X-VERIFY")] string signatureHeader)
        {
            await _paymentService.HandleWebhookAsync(payload, signatureHeader);
            return Ok(new { success = true });
        }
    }
}
