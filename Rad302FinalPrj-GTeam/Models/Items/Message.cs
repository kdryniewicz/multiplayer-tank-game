using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Rad302FinalPrj_GTeam.Models.Items
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "MessageID")]
        public int ID { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }
    }
}