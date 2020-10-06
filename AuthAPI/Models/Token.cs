using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPI.Models {
    public class Token {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public long ExpiryDate { get; set; }
        [Required]
        public long IssuedAt { get; set; }
        [NotMapped]
        public string JwtToken { get; set; }
    }
}