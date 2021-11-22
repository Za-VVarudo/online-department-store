using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject.Models
{
    public partial class OrderObject
    {
        public OrderObject()
        {
            OrderDetails = new HashSet<OrderDetailObject>();
        }
        [Key]
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal? Freight { get; set; }

        public virtual MemberObject Member { get; set; }
        public virtual ICollection<OrderDetailObject> OrderDetails { get; set; }
    }
}
