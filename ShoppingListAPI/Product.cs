using System;
using System.Collections.Generic;

namespace ShoppingListAPI
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int ProductCategoryId { get; set; }
        public int ProductQuantity { get; set; }

        public Product(string v1, int v2, int v3)
        {
            this.ProductName = v1;
            this.ProductCategoryId = v2;
            this.ProductQuantity = v3;
        }
        public Product()
        {
        }

        //public virtual ProductCategory ProductCategory { get; set; } = null!;
    }
}
