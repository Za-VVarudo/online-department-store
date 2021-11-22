using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWebApp.Models
{
    public class OrderModel
    {
        [Required]
        public DateTime RequiredDate { set; get; }
        [Required]
        public DateTime ShippedDate { set; get; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Freight { set; get; }
    }

    public class OrderInsertModel : OrderModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int MemberId { set; get; }
    }
    public class OrderUpdateModel : OrderModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int OrderId { set; get; }
        [Required]
        public DateTime OrderDate { set; get; }
    }
}
