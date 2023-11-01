using System;
using System.Collections.Generic;

namespace GadgetStore.DATA.EF.Models
{
    //partial is kind of like abstract. It means it *CAN* be an incomplete implementation.
    //nothing to do with inheritance.
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
