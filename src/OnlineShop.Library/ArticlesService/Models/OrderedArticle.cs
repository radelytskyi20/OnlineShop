﻿using OnlineShop.Library.Common.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using OnlineShop.Library.OrdersService.Models;
using System.Text.Json.Serialization;

namespace OnlineShop.Library.ArticlesService.Models
{
    public class OrderedArticle : IIdentifiable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Column(TypeName = "numeric(12, 4)"), Required]
        public decimal Price { get; set; }

        [Column(TypeName = "int"), Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [NotMapped] //not mapped to the database => ef core will ignore this property
        public decimal Total => Price * Quantity;

        [Required]
        public string PriceListName { get; set; }

        [Column(TypeName = "datetime2"), Required]
        public DateTime ValidFrom { get; set; }

        [Column(TypeName = "datetime2"), Required]
        public DateTime ValidTo { get; set; }

        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
