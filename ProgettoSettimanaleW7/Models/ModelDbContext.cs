using System;
using System.Data.Entity;
using System.Linq;

namespace ProgettoSettimanaleW7.Models
{
    public class ModelDbContext : DbContext
    {
        public DbSet<Articoli> Articoli { get; set; }
        public DbSet<Utenti> Utenti { get; set; }
        public DbSet<Ordini> Ordini { get; set; }

        public DbSet<DettagliOrdini> DettagliOrdini { get; set; }

        public ModelDbContext()
            : base("name=ModelDbContext")
        {

        }

     
    }

}