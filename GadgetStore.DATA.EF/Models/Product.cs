﻿using System;
using System.Collections.Generic;

namespace GadgetStore.DATA.EF.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public string? ProductDescription { get; set; }
        public short UnitsInStock { get; set; }
        public short UnitsOnOrder { get; set; }
        public bool IsDiscontinued { get; set; }
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public string? ProductImage { get; set; }

        //Category here is made nullable because it causes issues with object creation later on.
        public virtual Category? Category { get; set; } 
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
