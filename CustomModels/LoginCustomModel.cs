using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRMSWithTheme.CustomModels
{
    public class LoginCustomModel
    {
        [Required(ErrorMessage = "Email Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._]+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$", ErrorMessage = "*Email is not valid*")]
        //[EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
    }
}