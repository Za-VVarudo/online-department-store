using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWebApp.Models
{
    public class OrderDetailModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int OrderId { set; get; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { set; get; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { set; get; }
        [Required]
        [Range(0, 1)]
        public double Discount { set; get; }
    }
}
