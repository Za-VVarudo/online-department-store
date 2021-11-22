using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWebAPI.Models
{
    public class MemberModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { set; get; }
        [Required]
        [MaxLength(40)]
        public string CompanyName { set; get; }
        [Required]
        [MaxLength(15)]
        public string City { set; get; }
        [Required]
        [MaxLength(15)]
        public string Country { set; get; }
    }
    public class MemberUpdateModel : MemberModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int MemberId { set; get; }
    }
    public class MemberInsertModel : MemberModel
    {
        [Required]
        [MaxLength(30), MinLength(6)]
        public string Password { set; get; }
    }
}
