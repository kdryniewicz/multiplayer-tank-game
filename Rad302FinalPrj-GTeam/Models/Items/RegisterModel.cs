using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Rad302FinalPrj_GTeam.Models.Items
{
    class RegisterModel
    {
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}