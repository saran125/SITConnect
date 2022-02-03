using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace SITConnect.Models
{
    public class User
    {

     
        [Key]
        public string Id { get; set; }

        public string Fname { get; set; }
        
        public string Lname { get; set; }
       
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "It is not a valid email address")]
        public string Email { get; set; }


        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
            ErrorMessage = "Password Need to be at least 12 characters, combination of lower case, upper case, numbers & special characters")]
        public string Password { get; set; }
        [MinLength(16)]
        public string Card_number { get; set; }
        public string CVV { get; set; }
        [DataType(DataType.Date)]
        public DateTime Expiry_date { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }

        public string Role { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [FileExtensions(Extensions = "jpg, jpeg, png", ErrorMessage = "Provide a valid File Type (JPG/PNG)")]
        [DataType(DataType.Upload)]
        public string Photo { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime LockoutEnd { get; set; }
        public int LoginFail { get; set; }
        public bool LockedoutEnabled { get; set; }
        public DateTime Password_Changed_Time { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }

    }
}
