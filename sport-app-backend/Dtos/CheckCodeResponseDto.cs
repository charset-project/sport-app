using sport_app_backend.Models.Account;

namespace sport_app_backend.Dtos;

public class CheckCodeResponseDto
{
    /// whene code is true 
    public bool IsSuccess { get; set; }
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
    public TypeOfUser TypeOfUser { get; set; }
    public string? Message { get; set; }
}
