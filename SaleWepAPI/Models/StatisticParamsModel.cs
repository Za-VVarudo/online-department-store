using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWebAPI.Models
{
    public class StatisticParamsModel
    {
        [Required]
        public DateTime From { set; get; } = DateTime.Today.Date;
        [Required]
        public DateTime To { set; get; } = DateTime.Today.Date;
    }
}
