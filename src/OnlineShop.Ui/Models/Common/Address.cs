using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Ui.Models.Common
{
    public class Address
    {
        public Guid Id { get; set; }

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
    }
}
