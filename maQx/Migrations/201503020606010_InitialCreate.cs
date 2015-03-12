namespace maQx.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessLevels",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        UserId = c.String(nullable: false, maxLength: 128),
                        DivisionKey = c.String(nullable: false, maxLength: 40),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.DivisionKey)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => new { t.UserId, t.DivisionKey }, unique: true, name: "IX_AccessLevel");
            
            CreateTable(
                "dbo.Divisions",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Code = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Plant_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Plants", t => t.Plant_Key)
                .Index(t => t.Plant_Key);
            
            CreateTable(
                "dbo.Plants",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Code = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        Location = c.String(nullable: false, maxLength: 50),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Organization_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Organizations", t => t.Organization_Key)
                .Index(t => t.Organization_Key);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Code = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        Domain = c.String(nullable: false, maxLength: 50),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key)
                .Index(t => t.Code, unique: true, name: "IX_OrganizationCode")
                .Index(t => t.Domain, unique: true);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.String(),
                        Firstname = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AdminRegistrationBases",
                c => new
                    {
                        StepCode = c.String(nullable: false, maxLength: 32),
                        Email = c.String(nullable: false, maxLength: 100),
                        ResendActivity = c.Boolean(nullable: false),
                        ConfirmationCode = c.String(nullable: false),
                        Role = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StepCode)
                .ForeignKey("dbo.IntilizationSteps", t => t.StepCode)
                .Index(t => t.StepCode);
            
            CreateTable(
                "dbo.IntilizationSteps",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 32),
                        Mode = c.String(nullable: false),
                        Auth = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.Administrators",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        OrganizationKey = c.String(nullable: false, maxLength: 40),
                        Role = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationKey)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => new { t.UserId, t.OrganizationKey }, unique: true, name: "IX_OrganizationAdministrator");
            
            CreateTable(
                "dbo.DepartmentMenus",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        DepartmentKey = c.String(nullable: false, maxLength: 40),
                        MenuID = c.String(nullable: false, maxLength: 128),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Departments", t => t.DepartmentKey)
                .ForeignKey("dbo.Menus", t => t.MenuID)
                .Index(t => new { t.DepartmentKey, t.MenuID }, unique: true, name: "IX_DivisionMenu");
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Name = c.String(nullable: false),
                        Access = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Division_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.Division_Key)
                .Index(t => t.Division_Key);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 50),
                        Access = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        IsMappable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.DepartmentUsers",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Department_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Departments", t => t.Department_Key)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId, unique: true, name: "IX_DivisionUser")
                .Index(t => t.Department_Key);
            
            CreateTable(
                "dbo.Dies",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        MaxSink = c.Int(nullable: false),
                        MaxCount = c.Int(nullable: false),
                        Tolerance = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Division_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.Division_Key)
                .Index(t => t.Division_Key);
            
            CreateTable(
                "dbo.Invites",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Username = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 100),
                        Role = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Organization_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Organizations", t => t.Organization_Key)
                .Index(t => t.Username, unique: true)
                .Index(t => t.Organization_Key);
            
            CreateTable(
                "dbo.MenuAccesses",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        UserId = c.String(nullable: false, maxLength: 128),
                        DepartmentMenuKey = c.String(nullable: false, maxLength: 40),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.DepartmentMenus", t => t.DepartmentMenuKey)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => new { t.UserId, t.DepartmentMenuKey }, unique: true, name: "IX_MenuAccess");
            
            CreateTable(
                "dbo.Processes",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        ValidateRawMaterial = c.Boolean(nullable: false),
                        Code = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Division_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.Division_Key)
                .Index(t => t.Division_Key);
            
            CreateTable(
                "dbo.ProductRawMaterials",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        RawMaterialKey = c.String(nullable: false, maxLength: 40),
                        ProductKey = c.String(nullable: false, maxLength: 40),
                        Quantity = c.Double(nullable: false),
                        InputQuantity = c.Double(nullable: false),
                        SelectedUnit = c.Int(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Products", t => t.ProductKey)
                .ForeignKey("dbo.RawMaterials", t => t.RawMaterialKey)
                .Index(t => new { t.RawMaterialKey, t.ProductKey }, unique: true, name: "IX_ProductRawMaterial");
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Code = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Division_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.Division_Key)
                .Index(t => t.Division_Key);
            
            CreateTable(
                "dbo.RawMaterials",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        Unit = c.Int(nullable: false),
                        Measurement = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Division_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.Division_Key)
                .Index(t => t.Division_Key);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Tools",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        MaxCount = c.Int(nullable: false),
                        Tolerance = c.Int(nullable: false),
                        Code = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                        Division_Key = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Divisions", t => t.Division_Key)
                .Index(t => t.Division_Key);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tools", "Division_Key", "dbo.Divisions");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProductRawMaterials", "RawMaterialKey", "dbo.RawMaterials");
            DropForeignKey("dbo.RawMaterials", "Division_Key", "dbo.Divisions");
            DropForeignKey("dbo.ProductRawMaterials", "ProductKey", "dbo.Products");
            DropForeignKey("dbo.Products", "Division_Key", "dbo.Divisions");
            DropForeignKey("dbo.Processes", "Division_Key", "dbo.Divisions");
            DropForeignKey("dbo.MenuAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MenuAccesses", "DepartmentMenuKey", "dbo.DepartmentMenus");
            DropForeignKey("dbo.Invites", "Organization_Key", "dbo.Organizations");
            DropForeignKey("dbo.Dies", "Division_Key", "dbo.Divisions");
            DropForeignKey("dbo.DepartmentUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DepartmentUsers", "Department_Key", "dbo.Departments");
            DropForeignKey("dbo.DepartmentMenus", "MenuID", "dbo.Menus");
            DropForeignKey("dbo.DepartmentMenus", "DepartmentKey", "dbo.Departments");
            DropForeignKey("dbo.Departments", "Division_Key", "dbo.Divisions");
            DropForeignKey("dbo.Administrators", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Administrators", "OrganizationKey", "dbo.Organizations");
            DropForeignKey("dbo.AdminRegistrationBases", "StepCode", "dbo.IntilizationSteps");
            DropForeignKey("dbo.AccessLevels", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AccessLevels", "DivisionKey", "dbo.Divisions");
            DropForeignKey("dbo.Divisions", "Plant_Key", "dbo.Plants");
            DropForeignKey("dbo.Plants", "Organization_Key", "dbo.Organizations");
            DropIndex("dbo.Tools", new[] { "Division_Key" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.RawMaterials", new[] { "Division_Key" });
            DropIndex("dbo.Products", new[] { "Division_Key" });
            DropIndex("dbo.ProductRawMaterials", "IX_ProductRawMaterial");
            DropIndex("dbo.Processes", new[] { "Division_Key" });
            DropIndex("dbo.MenuAccesses", "IX_MenuAccess");
            DropIndex("dbo.Invites", new[] { "Organization_Key" });
            DropIndex("dbo.Invites", new[] { "Username" });
            DropIndex("dbo.Dies", new[] { "Division_Key" });
            DropIndex("dbo.DepartmentUsers", new[] { "Department_Key" });
            DropIndex("dbo.DepartmentUsers", "IX_DivisionUser");
            DropIndex("dbo.Menus", new[] { "Name" });
            DropIndex("dbo.Departments", new[] { "Division_Key" });
            DropIndex("dbo.DepartmentMenus", "IX_DivisionMenu");
            DropIndex("dbo.Administrators", "IX_OrganizationAdministrator");
            DropIndex("dbo.AdminRegistrationBases", new[] { "StepCode" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Organizations", new[] { "Domain" });
            DropIndex("dbo.Organizations", "IX_OrganizationCode");
            DropIndex("dbo.Plants", new[] { "Organization_Key" });
            DropIndex("dbo.Divisions", new[] { "Plant_Key" });
            DropIndex("dbo.AccessLevels", "IX_AccessLevel");
            DropTable("dbo.Tools");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RawMaterials");
            DropTable("dbo.Products");
            DropTable("dbo.ProductRawMaterials");
            DropTable("dbo.Processes");
            DropTable("dbo.MenuAccesses");
            DropTable("dbo.Invites");
            DropTable("dbo.Dies");
            DropTable("dbo.DepartmentUsers");
            DropTable("dbo.Menus");
            DropTable("dbo.Departments");
            DropTable("dbo.DepartmentMenus");
            DropTable("dbo.Administrators");
            DropTable("dbo.IntilizationSteps");
            DropTable("dbo.AdminRegistrationBases");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Organizations");
            DropTable("dbo.Plants");
            DropTable("dbo.Divisions");
            DropTable("dbo.AccessLevels");
        }
    }
}
