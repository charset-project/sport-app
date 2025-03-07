namespace sport_app_backend.Configuration
{
    public class AuthResult
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public bool IsNewUser { get; set; }
        public List<string> Errors { get; set; }
    }
}
