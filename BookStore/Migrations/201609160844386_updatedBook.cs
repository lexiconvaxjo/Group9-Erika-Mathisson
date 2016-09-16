namespace BookStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedBook : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Books", "ISBN", c => c.Long(nullable: false));
            DropColumn("dbo.Books", "Desctiption");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "Desctiption", c => c.String(nullable: false));
            AlterColumn("dbo.Books", "ISBN", c => c.Int(nullable: false));
            DropColumn("dbo.Books", "Description");
        }
    }
}
