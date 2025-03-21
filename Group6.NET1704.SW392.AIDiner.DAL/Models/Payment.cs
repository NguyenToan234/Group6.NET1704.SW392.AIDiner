﻿using System;
using System.Collections.Generic;

namespace Group6.NET1704.SW392.AIDiner.DAL.Models
{
    public partial class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int MethodId { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }

        public virtual PaymentMethod Method { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
