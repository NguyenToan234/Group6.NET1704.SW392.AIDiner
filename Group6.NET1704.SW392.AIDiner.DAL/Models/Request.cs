﻿using System;
using System.Collections.Generic;

namespace Group6.NET1704.SW392.AIDiner.DAL.Models
{
    public partial class Request
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TypeId { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string Status { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;
        public virtual RequestType Type { get; set; } = null!;
    }
}
