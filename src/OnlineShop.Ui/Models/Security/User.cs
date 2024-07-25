using OnlineShop.Ui.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Ui.Models.Security
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;
        public Address DefaultAddress { get; set; } = new();
        public Address DeliveryAddress { get; set; } = new();
    }
}
