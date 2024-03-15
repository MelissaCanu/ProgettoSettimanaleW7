namespace ProgettoSettimanaleW7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateTimeToDateTime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ordinis", "DataOrdine", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ordinis", "DataOrdine", c => c.DateTime(nullable: false));
        }
    }
}
