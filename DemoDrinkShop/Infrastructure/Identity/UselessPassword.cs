using System.ComponentModel.DataAnnotations;

namespace DemoDrinkShop.Infrastructure.Identity
{
    public class UselessPassword
    {
        [Key]
        public string HashedValue { get; set; }

        public UselessPassword() { }
        public UselessPassword(string hashed) => this.HashedValue = hashed;
    }

}
