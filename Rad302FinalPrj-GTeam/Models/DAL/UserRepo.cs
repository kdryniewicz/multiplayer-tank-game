using Rad302FinalPrj_GTeam.Models.Items;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Rad302FinalPrj_GTeam.Models.DAL
{
    public class UserRepo : IUser, IDisposable
    {
        private GeneralDbContext context;

        public UserRepo(GeneralDbContext context)
        {
            this.context = context;
        }

        public void Dispose()
        {
            this.Dispose();
        }

        public async Task<IList<User>> GetUsers()
        {
            return await context.Users.ToListAsync();
        }

        public async Task<User> AddUser(User u)
        {
            context.Users.Add(u);
            await context.SaveChangesAsync();
            return u;
        }

        public async Task<User> DeleteUser(int ID)
        {
            User user = await context.Users.FindAsync(ID);
            if (user == null)
            {
                return null;
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return user;
        }
        private bool UserExists(int id)
        {
            return context.Users.Count(u => u.UserID == id) > 0;
        }

        public async Task<Player> GetPlayer(int UserID)
        {
            //Looks for User to that has the player data associated with it
            User u = await context.Users.FindAsync(UserID);
            if (u == null)
            {
                return null;
            }
            else
            {
                //User was found so now to find Player associated with it and return it or return null.
                Player p = await context.Players.FindAsync(u.PlayerID);
                if (p == null)
                {
                    return null;
                }
                return p;
            }
        }

        public async Task<IList<Player>> GetPlayers()
        {
            return await context.Players.ToListAsync();
        }

        public async Task<User> UpdatePlayer(int UserID, Player player)
        {
            //Looks for User to that has the player data associated with it
            User u = await context.Users.FindAsync(UserID);
            if (u == null)
            {
                return null;
            }
            else
            {
                //User was found so now to update Player associated with it and return it or return null.
                Player p = await context.Players.FindAsync(u.PlayerID);
                if (p == null)
                {
                    return null;
                }
                context.Players.Remove(p);
                context.Players.Add(player);
                await context.SaveChangesAsync();
                return u;
            }
        }

        public async Task<Player> DeletePlayer(Player p)
        {
            Player player = await context.Players.FindAsync(p);
            if (player == null)
            {
                return null;
            }
            context.Players.Remove(player);
            await context.SaveChangesAsync();

            return player;
        }


        public async Task<Player> AddPlayer(Player p)
        {
            context.Players.Add(p);
            await context.SaveChangesAsync();

            return p;
        }

        async Task<User> IRepository<User>.PutEntity(User Entity)
        {
            context.Entry(Entity).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
                return Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(Entity.UserID))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

        }

        async Task<User> IRepository<User>.PostEntity(User Entity)
        {
            context.Users.Add(Entity);
            await context.SaveChangesAsync();
            return Entity;
        }

        async Task<User> IRepository<User>.GetEntity(int id)
        {
            User user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            return user;
        }

        async Task<IList<User>> IRepository<User>.getEntities()
        {
            return await context.Users.ToListAsync();
        }

        async Task<User> IRepository<User>.delete(int id)
        {
            User user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return user;

        }
    }
}