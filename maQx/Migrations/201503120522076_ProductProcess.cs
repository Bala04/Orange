namespace maQx.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ProductProcess : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductProcesses", "ProcessKey", "dbo.Processes");
            DropForeignKey("dbo.ProductProcesses", "ProductKey", "dbo.Products");
            DropIndex("dbo.ProductProcesses", "IX_ProductProcess");
            DropTable("dbo.ProductProcesses");
        }

        public override void Down()
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
                .PrimaryKey(t => t.Key);

            CreateIndex("dbo.ProductProcesses", new[] { "ProductKey", "ProcessKey", "Order" }, unique: true, name: "IX_ProductProcess");
            AddForeignKey("dbo.ProductProcesses", "ProductKey", "dbo.Products", "Key");
            AddForeignKey("dbo.ProductProcesses", "ProcessKey", "dbo.Processes", "Key");
        }
    }
}
