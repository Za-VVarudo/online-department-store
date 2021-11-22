using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWebAPI.Models
{
    public class ProductModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int CategoryId { set; get; }
        [Required]
        [MaxLength(40)]
        public string ProductName { set; get; }
        [Required]
        [MaxLength(20)]
        public string Weight { set; get; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { set; get; }
        [Required]
        [Range(0, int.MaxValue)]
        public int UnitsInStock { set; get; }
    }
    public class ProductUpdateModel : ProductModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { set; get; }
    }
}
