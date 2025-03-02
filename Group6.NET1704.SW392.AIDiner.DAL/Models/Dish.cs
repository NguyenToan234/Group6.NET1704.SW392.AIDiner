  using System;
using System.Collections.Generic;

namespace Group6.NET1704.SW392.AIDiner.DAL.Models
{
    public partial class Dish
    {
        public Dish()
        {
            DishIngredients = new HashSet<DishIngredient>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int SoldCount { get; set; }
        public string Status { get; set; } = null!;
        public string? Image { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int RestaurantId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<DishIngredient> DishIngredients { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
