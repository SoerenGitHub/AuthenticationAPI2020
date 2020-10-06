using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Dtos {
    public class UserLoginDto {
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }
        [Required]
        [MaxLength(250)]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}