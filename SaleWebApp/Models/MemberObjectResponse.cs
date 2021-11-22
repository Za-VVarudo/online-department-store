using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWepApp.Models
{
    public class MemberObjectResponse
    {
        public MemberObject Member { set; get; }
        public string Role { set; get; }
    }
}
