namespace sport_app_backend.Models.Account
{
    public class RefreshTokenModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
