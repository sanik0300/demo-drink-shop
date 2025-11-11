using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoDrinkShop.Infrastructure.Identity
{
    public class VerificationCode
    {
        public static int GenerateValue()
        {
            return Random.Shared.Next(1000, 10_000);
        }

        [Key]
        [Required]
        public string Email { get; set; }
        
        [Range(1000, 9999)]
        public int Value { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;

        [NotMapped]
        public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
    }
}
