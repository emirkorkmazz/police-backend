using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Police.Data
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string HashedUserIdentityNo { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public int BanCount { get; set; }
        public string BanReason { get; set; }
        public DateTime? BanEndTime { get; set; }
        public bool Status { get; set; }
        public string Token { get; set; }
        public string? CompanyIdsJson { get; set; }
        public DateTime LicenseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


    }
}
