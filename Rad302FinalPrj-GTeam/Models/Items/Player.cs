using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Rad302FinalPrj_GTeam.Models.Items
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "PlayerID")]
        public int PlayerID { get; set; }

        [Display(Name = "DisplayName")]
        public string DisplayName { get; set; }

        [Display(Name = "Score")]
        public int Score { get; set; }
    }
}