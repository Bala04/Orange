namespace maQx.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using maQx.Utilities;
    using maQx.Models;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<maQx.Models.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "maQx.Models.AppContext";
        }

        protected override void Seed(maQx.Models.AppContext context)
        {
            var Manager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            typeof(Roles).GetConstantValues<string>().ForEach(x =>
            {
                if (!context.Roles.AnyAsync(r => r.Name == x).Result)
                {
                    Manager.Create(new IdentityRole { Name = x });
                }
            });

            // Default and Fixed Menus for the Application
            (new List<Menus> {
                new Menus { ID = "Organizations", Name = "Organizations", Access = Roles.AppAdmin, Order = 1, IsMappable = false },
                new Menus { ID = "Invites", Name = "Invites", Access = Roles.Inviter, Order = 2, IsMappable = false },
                new Menus { ID = "Administrators", Name = "People", Access = Roles.Inviter, Order = 3, IsMappable = false },
                new Menus { ID = "Plants", Name = "Plants", Access = Roles.SysAdmin, Order = 4, IsMappable = false },
                new Menus { ID = "Divisions", Name = "Divisions", Access = Roles.SysAdmin, Order = 5, IsMappable = false },
                new Menus { ID = "Departments", Name = "Departments", Access = Roles.SysAdmin, Order = 6, IsMappable = false },
                new Menus { ID = "DepartmentMenus", Name = "Department - Menu", Access = Roles.SysAdmin, Order = 7, IsMappable = false },
                new Menus { ID = "DepartmentUsers", Name = "Department - User", Access = Roles.SysAdmin, Order = 8, IsMappable = false },
                new Menus { ID = "AccessLevel", Name = "Access Levels", Access = Roles.SysAdmin, Order = 9, IsMappable = false },
                new Menus { ID = "RawMaterials", Name = "Raw Materials", Access = Roles.AppUser, Order = 10, IsMappable = false },
                new Menus { ID = "Products", Name = "Products", Access = Roles.AppUser, Order = 11, IsMappable = false },
                new Menus { ID = "Process", Name = "Process", Access = Roles.AppUser, Order = 12, IsMappable = false },
                new Menus { ID = "Tools", Name = "Tools", Access = Roles.AppUser, Order = 13, IsMappable = false },
                new Menus { ID = "Dies", Name = "Dies", Access = Roles.AppUser, Order = 14, IsMappable = false },
                new Menus { ID = "ProductRawMaterials", Name= "Product - Raw Material", Access = Roles.AppUser, Order = 15, IsMappable = false },
                new Menus { ID = "ProductProcess", Name= "Product - Process", Access = Roles.AppUser, Order = 16, IsMappable = false },
                new Menus { ID = "ProductProcessTools", Name= "Product - Process - Tool", Access = Roles.AppUser, Order = 17, IsMappable = false },
                new Menus { ID = "ProductProcessDies", Name= "Product - Process - Die", Access = Roles.AppUser, Order = 18, IsMappable = false },
                new Menus { ID = "Machines", Name= "Machines", Access = Roles.AppUser, Order = 19, IsMappable = false }
            }).ForEach(x => { context.Menus.AddOrUpdate(x); });
        }
    }
}
