using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group6.NET1704.SW392.AIDiner.Common.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
         public int TableId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool PaymentStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Status { get; set; } = null!;

        public string? TableName { get; set; }

        public List<Order_OrderDetailDTO> Dishes { get; set; } = new List<Order_OrderDetailDTO>();

    }
}
