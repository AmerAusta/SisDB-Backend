namespace BusinessLayer.User.Login.LoginDto 
{
    public class TokenResponseDto
    {
        public bool IsAuthenticated { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}