using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Police.Data
{
    public class PoliciesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
        public string LicenseNumber { get; set; }
        public string PlateNumber { get; set; }
        public string ShasiNumber { get; set; }
        public double PolicyAmount { get; set; }
        public double PolicyRate { get; set; }
        public string Notes { get; set; }
        public int ReminderDays { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
