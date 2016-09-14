namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedTablesForDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Author = c.String(nullable: false),
                        Desctiption = c.String(nullable: false),
                        ISBN = c.Int(nullable: false),
                        NumberInStock = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderRows",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Double(nullable: false),
                        NoOfItem = c.Int(nullable: false),
                        BookPurchase_Id = c.Int(nullable: false),
                        order_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookPurchase_Id, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.order_Id, cascadeDelete: true)
                .Index(t => t.BookPurchase_Id)
                .Index(t => t.order_Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        UserBuyer_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserBuyer_Id, cascadeDelete: true)
                .Index(t => t.UserBuyer_Id);
            
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Address", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "ZipCode", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "City", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderRows", "order_Id", "dbo.Orders");
            DropForeignKey("dbo.Orders", "UserBuyer_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrderRows", "BookPurchase_Id", "dbo.Books");
            DropIndex("dbo.Orders", new[] { "UserBuyer_Id" });
            DropIndex("dbo.OrderRows", new[] { "order_Id" });
            DropIndex("dbo.OrderRows", new[] { "BookPurchase_Id" });
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "ZipCode");
            DropColumn("dbo.AspNetUsers", "Address");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderRows");
            DropTable("dbo.Books");
        }
    }
}
