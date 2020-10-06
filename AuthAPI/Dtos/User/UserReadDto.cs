using AuthAPI.Models;

namespace AuthAPI.Dtos {
    public class UserReadDto {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Forename { get; set; }
        public string Email { get; set; }
    }
}