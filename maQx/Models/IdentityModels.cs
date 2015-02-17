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
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc.Filters;
using System.Web.Mvc;


namespace maQx.Models
{
    /// <summary>
    ///
    /// </summary>
    public class Roles
    {
        /// <summary>
        /// The application admin
        /// </summary>
        public const string AppAdmin = "AppAdmin";
        /// <summary>
        /// The system admin
        /// </summary>
        public const string SysAdmin = "SysAdmin";
        /// <summary>
        /// The application user
        /// </summary>
        public const string AppUser = "AppUser";
        /// <summary>
        /// The inviter
        /// </summary>
        public const string Inviter = "Inviter";
        /// <summary>
        /// The organization
        /// </summary>
        public const string Create = "Create";
        /// <summary>
        /// The plant
        /// </summary>
        public const string Edit = "Edit";
        /// <summary>
        /// The division
        /// </summary>
        public const string Delete = "Delete";
    }

    public class AccessAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        protected override bool AuthorizeCore(HttpContextBase Context)
        {
            return Context.User.Identity.IsAuthenticated ? Context.User.HasMenuAccess(Context.Request.RequestContext.RouteData.Values["controller"].ToString()) : false;
        }

        public override void OnAuthorization(AuthorizationContext context)
        {
            base.OnAuthorization(context);
        }
    }

    /// <summary>
    ///
    /// </summary>
    ///
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string Firstname { get; set; }

        /// <summary>
        /// Generates the user identity asynchronous.
        /// </summary>
        /// <param name="manager">The manager.</param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            Organization Organization = null;
            DepartmentUser DepartmentUsers = null;

            using (var db = new AppContext())
            {
                var Entity = await db.Administrators.Include("Organization").SingleOrDefaultAsync(x => x.User.Id == this.Id);
                Organization = Entity == null ? null : Entity.Organization;
                DepartmentUsers = await db.DepartmentUsers.Include(x => x.Department.Division).Where(x => x.User.Id == this.Id).FirstOrDefaultAsync();
            }

            userIdentity.AddClaim(new Claim("Firstname", Firstname));
            userIdentity.AddClaim(new Claim("Code", Code));
            userIdentity.AddClaim(new Claim("Organization.Key", Organization == null ? "" : Organization.Key));

            if (DepartmentUsers != null)
            {
                userIdentity.AddClaim(new Claim("Division.Key", DepartmentUsers.Department.Division.Key));
                userIdentity.AddClaim(new Claim("Department.Key", DepartmentUsers.Department.Key));
                userIdentity.AddClaim(new Claim("Department.Name", DepartmentUsers.Department.Name));
            }

            return userIdentity;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserManager"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        /// <summary>
        /// Creates the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
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

    /// <summary>
    ///
    /// </summary>
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSignInManager" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="authenticationManager">The authentication manager.</param>
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        /// <summary>
        /// Creates the user identity asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
        /// <summary>
        /// Creates the specified options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class AppContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppContext"/> class.
        /// </summary>
        public AppContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public static AppContext Create()
        {
            return new AppContext();
        }

        /// <summary>
        /// Before the save.
        /// </summary>
        /// <exception cref="Exception">Unauthorized Request. Authentication credentials were expired or incorrect.</exception>
        private void BeforeSave()
        {
            var entities = ChangeTracker.Entries();

            foreach (var entity in entities)
            {
                if (entity.Entity is AppBaseStamp)
                {
                    // BUG: var Id = HttpContext.Current != null && HttpContext.Current.User != null ? HttpContext.Current.User.Identity.GetUserId() : "Anonymous";
                    // FIX #9: The should be authenticated to Insert Or Modify the Model with AppBaseStamp as base. 29/01/2015
                    // #9: Validation error while registering new user. Module App/Register
                    if (entity.State == EntityState.Added || entity.State == EntityState.Modified)
                    {
                        if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
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
                        else
                        {
                            throw new Exception("Unauthorized Request. Authentication credentials were expired or incorrect.");
                        }
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

        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        /// The task result contains the number of state entries written to the underlying database. This can include
        /// state entries for entities and/or relationships. Relationship state entries are created for
        /// many-to-many relationships and relationships where there is no foreign key property
        /// included in the entity class (often referred to as independent associations).
        /// </returns>
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                BeforeSave();
                return await base.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the underlying database. This can include
        /// state entries for entities and/or relationships. Relationship state entries are created for
        /// many-to-many relationships and relationships where there is no foreign key property
        /// included in the entity class (often referred to as independent associations).
        /// </returns>
        public override int SaveChanges()
        {
            BeforeSave();
            return base.SaveChanges();
        }

        /// <summary>
        /// Gets or sets the menus.
        /// </summary>
        /// <value>
        /// The menus.
        /// </value>
        public DbSet<Menus> Menus { get; set; }
        /// <summary>
        /// Gets or sets the initialize step.
        /// </summary>
        /// <value>
        /// The initialize step.
        /// </value>
        public DbSet<IntilizationStep> InitStep { get; set; }
        /// <summary>
        /// Gets or sets the admin base.
        /// </summary>
        /// <value>
        /// The admin base.
        /// </value>
        public DbSet<AdminRegistrationBase> AdminBase { get; set; }
        /// <summary>
        /// Gets or sets the invites.
        /// </summary>
        /// <value>
        /// The invites.
        /// </value>
        public DbSet<Invite> Invites { get; set; }
        /// <summary>
        /// Gets or sets the administrators.
        /// </summary>
        /// <value>
        /// The administrators.
        /// </value>
        public DbSet<Administrator> Administrators { get; set; }
        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        public DbSet<Organization> Organizations { get; set; }
        /// <summary>
        /// Gets or sets the plants.
        /// </summary>
        /// <value>
        /// The plants.
        /// </value>
        public DbSet<Plant> Plants { get; set; }
        /// <summary>
        /// Gets or sets the divisions.
        /// </summary>
        /// <value>
        /// The divisions.
        /// </value>
        public DbSet<Division> Divisions { get; set; }
        /// <summary>
        /// Gets or sets the departments.
        /// </summary>
        /// <value>
        /// The departments.
        /// </value>
        public DbSet<Department> Departments { get; set; }
        /// <summary>
        /// Gets or sets the department menus.
        /// </summary>
        /// <value>
        /// The department menus.
        /// </value>
        public DbSet<DepartmentMenu> DepartmentMenus { get; set; }
        /// <summary>
        /// Gets or sets the department users.
        /// </summary>
        /// <value>
        /// The department users.
        /// </value>
        public DbSet<DepartmentUser> DepartmentUsers { get; set; }
        /// <summary>
        /// Gets or sets the access levels.
        /// </summary>
        /// <value>
        /// The access levels.
        /// </value>
        public DbSet<AccessLevel> AccessLevels { get; set; }
        /// <summary>
        /// Gets or sets the menu access.
        /// </summary>
        /// <value>
        /// The menu access.
        /// </value>
        public DbSet<MenuAccess> MenuAccess { get; set; }
        /// <summary>
        /// Gets or sets the raw materials.
        /// </summary>
        /// <value>
        /// The raw materials.
        /// </value>
        public DbSet<RawMaterial> RawMaterials { get; set; }
        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        /// <value>
        /// The products.
        /// </value>
        public DbSet<Product> Products { get; set; }
        /// <summary>
        /// Gets or sets the product raw materials.
        /// </summary>
        /// <value>
        /// The product raw materials.
        /// </value>
        public DbSet<ProductRawMaterial> ProductRawMaterials { get; set; }
        /// <summary>
        /// Gets or sets the process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        public DbSet<Process> Process { get; set; }
        /// <summary>
        /// Gets or sets the tools.
        /// </summary>
        /// <value>
        /// The tools.
        /// </value>
        public DbSet<Tool> Tools { get; set; }
        /// <summary>
        /// Gets or sets the dies.
        /// </summary>
        /// <value>
        /// The dies.
        /// </value>
        public DbSet<Die> Dies { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class AppContextInitializer : DropCreateDatabaseIfModelChanges<AppContext>
    {
        /// <summary>
        /// Seeds the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
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
            }).ForEach(x => { context.Menus.Add(x); });

            base.Seed(context);
        }
    }
}