using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Police.Data
{
    public class ReferencesModel
    {
        [Key]
        public int ReferenceId { get; set; }
        public string NameSurname { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
