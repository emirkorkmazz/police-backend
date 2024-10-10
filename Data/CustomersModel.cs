using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Police.Data
{
    public class CustomersModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }  
        public bool IsPerson { get; set; }
        public string IdentityNumber { get; set; }
        public string TaxNumber { get; set; }
        public string TaxOffice { get; set; }
        public string NameSurname { get; set; }
        public string CompanyName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
