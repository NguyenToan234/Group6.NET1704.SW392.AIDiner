﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Group6.NET1704.SW392.AIDiner.DAL.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            DishIngredients = new HashSet<DishIngredient>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public virtual ICollection<DishIngredient> DishIngredients { get; set; }
    }
}