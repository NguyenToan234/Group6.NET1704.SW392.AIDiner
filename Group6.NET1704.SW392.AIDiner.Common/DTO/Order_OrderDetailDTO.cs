using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Common.DTO
{
    public class Order_OrderDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Image { get; set; } 
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } 
    }
}
