namespace ProgettoSettimanaleW7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIngredienti : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articolis", "Ingredienti", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articolis", "Ingredienti");
        }
    }
}
