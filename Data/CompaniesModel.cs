using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Police.Data
{
    public class CompaniesModel
    {
        [Key]
        public int CompanyId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
