namespace maQx.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductProcesses",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 40),
                        ProductKey = c.String(nullable: false, maxLength: 40),
                        ProcessKey = c.String(nullable: false, maxLength: 40),
                        Order = c.Int(nullable: false),
                        UserCreated = c.String(nullable: false),
                        UserModified = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        ActiveFlag = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Key)
                .ForeignKey("dbo.Processes", t => t.ProcessKey)
                .ForeignKey("dbo.Products", t => t.ProductKey)
                .Index(t => new { t.ProductKey, t.ProcessKey, t.Order }, unique: true, name: "IX_ProductProcess");
        }

        public override void Down()
        {
            DropForeignKey("dbo.ProductProcesses", "ProductKey", "dbo.Products");
            DropForeignKey("dbo.ProductProcesses", "ProcessKey", "dbo.Processes");
            DropIndex("dbo.ProductProcesses", "IX_ProductProcess");
            DropTable("dbo.ProductProcesses");
        }
    }
}
