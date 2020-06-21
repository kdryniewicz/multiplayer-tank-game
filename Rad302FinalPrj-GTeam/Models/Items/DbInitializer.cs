using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Migrations;

namespace Rad302FinalPrj_GTeam.Models.Items
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<GeneralDbContext>
    {
        protected override void Seed(GeneralDbContext context)
        {
            SeedUsers(context);


            base.Seed(context);
        }
        protected void SeedUsers(GeneralDbContext context)
        {
            context.Users.AddOrUpdate(u => u.UserName,
             new User[] {
                    new User { UserName = "UserOne", Password = new PasswordHasher().HashPassword("Password$1"), Email = "test@gmail.com", associatedPlayer = new Player() { DisplayName = "User One"} },
                    new User { UserName = "UserTwo", Password = new PasswordHasher().HashPassword("Password$1"), Email = "test2@yahoo.com", associatedPlayer = new Player() { DisplayName = "User Two"} },
             });
            context.SaveChanges();
        }
     
    }
}