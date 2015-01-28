// Copyright (c) IP Rings Ltd. All rights reserved.
// Version 2.0.1. Author: Prasanth <@prashanth702> 

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using maQx.Utilities;


namespace maQx.Models
{
    public class Roles
    {
        public const string AppAdmin = "AppAdmin";
        public const string SysAdmin = "SysAdmin";
        public const string AppUser = "AppUser";
        public const string Inviter = "Inviter";
        public const string Organization = "Organization";
        public const string Plant = "Plant";
        public const string Division = "Division";
    }

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Code { get; set; }
        public string Firstname { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            Organization Organization = null;

            using (var db = new AppContext())
            {
                var Entity = await db.Administrators.Include("Organization").SingleOrDefaultAsync(x => x.User.Id == this.Id);
                Organization = Entity == null ? null : Entity.Organization;
            }

            userIdentity.AddClaim(new Claim("Firstname", Firstname));
            userIdentity.AddClaim(new Claim("Code", Code));
            userIdentity.AddClaim(new Claim("Organization.Key", Organization == null ? "" : Organization.Key));
            userIdentity.AddClaim(new Claim("Organization.Name", Organization == null ? "" : Organization.Name));
            return userIdentity;
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<AppContext>()));

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        public AppContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public static AppContext Create()
        {
            return new AppContext();
        }

        private void UpdateDateTime()
        {
            var entities = ChangeTracker.Entries();

            foreach (var entity in entities)
            {
                if (entity.Entity is AppBaseStamp)
                {
                    var entityItem = ((AppBaseStamp)entity.Entity);

                    var Id = HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity.GetUserId() : "Anonymous";

                    if (entity.State == EntityState.Added)
                    {
                        entityItem.Key = String.IsNullOrWhiteSpace(entityItem.Key) ? Guid.NewGuid().ToString() : entityItem.Key;
                        entityItem.UserCreated = Id;
                        entityItem.UserModified = Id;
                    }
                    else if (entity.State == EntityState.Modified)
                    {
                        entityItem.UserModified = Id;
                    }
                }

                if (entity.Entity is DateTimeStamp)
                {
                    var entityItem = ((DateTimeStamp)entity.Entity);

                    if (entity.State == EntityState.Added)
                    {
                        entityItem.CreatedAt = DateTime.Now;
                        entityItem.UpdatedAt = DateTime.Now;
                        entityItem.ActiveFlag = true;
                    }
                    else if (entity.State == EntityState.Modified)
                    {
                        entityItem.UpdatedAt = DateTime.Now;
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                UpdateDateTime();
                return await base.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public override int SaveChanges()
        {
            UpdateDateTime();
            return base.SaveChanges();
        }

        public DbSet<IntilizationStep> InitStep { get; set; }
        public DbSet<AdminRegistrationBase> AdminBase { get; set; }
        public DbSet<Menus> Menus { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentMenu> DepartmentMenus { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<DepartmentUser> DepartmentUsers { get; set; }
    }

    public class AppContextInitializer : DropCreateDatabaseIfModelChanges<AppContext>
    {
        protected override void Seed(AppContext context)
        {
            var Manager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            typeof(Roles).GetConstantValues<string>().ForEach(x =>
            {
                if (!context.Roles.AnyAsync(r => r.Name == x).Result)
                {
                    Manager.Create(new IdentityRole { Name = x });
                }
            });

            (new List<Menus> {
                new Menus { ID = "Organizations", Name = "Organizations", Access = Roles.AppAdmin, Order = 1, IsMappable = false },
                new Menus { ID = "Invites", Name = "Invites", Access = Roles.Inviter, Order = 2, IsMappable = false },
                new Menus { ID = "Administrators", Name = "People", Access = Roles.Inviter, Order = 3, IsMappable = false },
                new Menus { ID = "Plants", Name = "Plants", Access = Roles.SysAdmin, Order = 4, IsMappable = false },
                new Menus { ID = "Divisions", Name = "Divisions", Access = Roles.SysAdmin, Order = 5, IsMappable = false },
                new Menus { ID = "Departments", Name = "Departments", Access = Roles.SysAdmin, Order = 6, IsMappable = false },
                new Menus { ID = "DepartmentMenus", Name = "Department - Menu", Access = Roles.SysAdmin, Order = 7, IsMappable = false },
                new Menus { ID = "DepartmentUsers", Name = "Department - User", Access = Roles.SysAdmin, Order = 8, IsMappable = false },
                new Menus { ID = "AppAccess", Name = "Access Levels", Access = Roles.SysAdmin, Order = 9, IsMappable = false },
            }).ForEach(x => { context.Menus.Add(x); });

            (new List<Department> {                
                new Department { ID = "Production", Name = "Production", Access = Roles.SysAdmin },
                new Department { ID = "Engineering", Name = "Engineering", Access = Roles.SysAdmin },
                new Department { ID = "Maintenance", Name = "Maintenance", Access = Roles.SysAdmin },
                new Department { ID = "Quality", Name = "Quality", Access = Roles.SysAdmin },
            }).ForEach(x => { context.Departments.Add(x); });

            base.Seed(context);
        }
    }
}