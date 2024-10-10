using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Police.Data
{
    public class ProductsModel
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductDays { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
