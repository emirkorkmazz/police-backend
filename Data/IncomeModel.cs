using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Police.Data
{
    public class IncomeModel
    {
        [Key]
        public int IncomeId { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double Amount { get; set; }
    }
}
