using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public PlayerDTO Player { get; set; }
        public override string ToString()
        {
            return string.Concat(UserID.ToString(), " ", UserName, " ", Email);
        }
    }

    public class PlayerDTO
    {
        public int PlayerID { get; set; }
        public string DisplayName { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            return string.Concat(PlayerID.ToString(), " ", DisplayName, " ", Score);
        }
    }

    public class RegisteredDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public override string ToString()
        {
            return string.Concat(UserName, " ");
        }
    }
}
