﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MyClassesShop.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public Product Product { get; set; }

        public Order Order { get; set; }
    }

}
