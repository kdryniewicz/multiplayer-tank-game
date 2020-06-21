using Rad302FinalPrj_GTeam.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rad302FinalPrj_GTeam.Models.DAL
{
    interface IUser : IRepository<User>
    {
        Task<IList<User>> GetUsers();
        Task<User> DeleteUser(int ID);
        Task<User> AddUser(User u);
        Task<User> UpdatePlayer(int UserID, Player player);

        Task<Player> GetPlayer(int UserID);
        Task<IList<Player>> GetPlayers();
        Task<Player> AddPlayer(Player P);
        Task<Player> DeletePlayer(Player P);
    }
}
