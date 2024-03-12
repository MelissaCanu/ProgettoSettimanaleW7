namespace ProgettoSettimanaleW7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RuoloDA : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Utentis", "Ruolo", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Utentis", "Ruolo", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
