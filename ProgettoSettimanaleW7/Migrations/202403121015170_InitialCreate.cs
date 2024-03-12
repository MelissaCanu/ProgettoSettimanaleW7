namespace ProgettoSettimanaleW7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articolis",
                c => new
                    {
                        IdArticolo = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        Immagine = c.String(maxLength: 200),
                        Prezzo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TempoConsegna = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdArticolo);
            
            CreateTable(
                "dbo.DettagliOrdinis",
                c => new
                    {
                        IdDettagliOrdine = c.Int(nullable: false, identity: true),
                        IdOrdine = c.Int(nullable: false),
                        IdArticolo = c.Int(nullable: false),
                        Quantita = c.Int(nullable: false),
                        PrezzoTotale = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.IdDettagliOrdine)
                .ForeignKey("dbo.Articolis", t => t.IdArticolo, cascadeDelete: true)
                .ForeignKey("dbo.Ordinis", t => t.IdOrdine, cascadeDelete: true)
                .Index(t => t.IdOrdine)
                .Index(t => t.IdArticolo);
            
            CreateTable(
                "dbo.Ordinis",
                c => new
                    {
                        IdOrdine = c.Int(nullable: false, identity: true),
                        IdUtente = c.Int(nullable: false),
                        Indirizzo = c.String(nullable: false, maxLength: 100),
                        IsEvaso = c.Boolean(nullable: false),
                        Note = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.IdOrdine)
                .ForeignKey("dbo.Utentis", t => t.IdUtente, cascadeDelete: true)
                .Index(t => t.IdUtente);
            
            CreateTable(
                "dbo.Utentis",
                c => new
                    {
                        IdUtente = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        Ruolo = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdUtente);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DettagliOrdinis", "IdOrdine", "dbo.Ordinis");
            DropForeignKey("dbo.Ordinis", "IdUtente", "dbo.Utentis");
            DropForeignKey("dbo.DettagliOrdinis", "IdArticolo", "dbo.Articolis");
            DropIndex("dbo.Ordinis", new[] { "IdUtente" });
            DropIndex("dbo.DettagliOrdinis", new[] { "IdArticolo" });
            DropIndex("dbo.DettagliOrdinis", new[] { "IdOrdine" });
            DropTable("dbo.Utentis");
            DropTable("dbo.Ordinis");
            DropTable("dbo.DettagliOrdinis");
            DropTable("dbo.Articolis");
        }
    }
}
