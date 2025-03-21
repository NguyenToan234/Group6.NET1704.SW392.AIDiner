﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Common.DTO.AdminDTO
{
    public class DishAdminDTO
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Image {  get; set; }
        public int SoldCount { get; set; }
        public string Status { get; set; }
        public int? RestaurantId { get; set; }
        public string? RestaurantName { get; set; } 
        public List<IngredientDTO> Ingredients { get; set; }
    }
}
