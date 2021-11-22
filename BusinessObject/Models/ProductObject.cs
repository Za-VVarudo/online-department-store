using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject.Models
{
    public partial class ProductObject
    {
        public ProductObject()
        {
            OrderDetails = new HashSet<OrderDetailObject>();
        }
        [Key]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string Weight { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }

        public virtual ICollection<OrderDetailObject> OrderDetails { get; set; }
    }
}
