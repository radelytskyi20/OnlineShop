using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Library.Common.Interfaces;

namespace OnlineShop.Library.OrdersService.Models
{
    public class OrderStatusTrack : IIdentifiable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column(TypeName = "datetime2")]
        [Required]
        public DateTime Assigned { get; set; }

        [Required]
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        [Required]
        [ForeignKey("OrderStatus")]
        public int OrderStatusId { get; set; }

        public Order Order { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
