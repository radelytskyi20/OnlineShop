using OnlineShop.Library.Common.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Library.Constants;

namespace OnlineShop.Library.ArticlesService.Models
{
    public class PriceList : IIdentifiable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [ForeignKey("Article")]
        public Guid ArticleId { get; set; }

        [Column(TypeName = "numeric(12, 4)")] //12 digits, 4 of them after the decimal point
        public decimal Price { get; set; }

        [Required]
        public string Name { get; set; } = PriceListNames.DEFAULT;

        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime ValidFrom { get; set; }

        [Column(TypeName = "datetime2"), Required]
        public DateTime ValidTo { get; set; }
    }
}
