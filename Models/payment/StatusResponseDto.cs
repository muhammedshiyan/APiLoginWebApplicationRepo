namespace APiLoginWebApplication.Models
{
    public class StatusResponseDto
    {
        public bool Success { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
    }

}
