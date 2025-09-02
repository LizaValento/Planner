namespace Application.DTOs
{
    public class RefreshTokenModel
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
    }
}
