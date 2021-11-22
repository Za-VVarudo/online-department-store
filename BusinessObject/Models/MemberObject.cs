using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject.Models
{
    public partial class MemberObject
    {
        public MemberObject()
        {
            Orders = new HashSet<OrderObject>();
        }
        [Key]
        public int MemberId { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Password { get; set; }
        public virtual ICollection<OrderObject> Orders { get; set; }
    }
}
