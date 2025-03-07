using System.ComponentModel.DataAnnotations;

namespace sport_app_backend.Models.DTO.Requests
{
    public class UserRegistrationDTO
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public string VerificationCode { get; set; }
    }
}
