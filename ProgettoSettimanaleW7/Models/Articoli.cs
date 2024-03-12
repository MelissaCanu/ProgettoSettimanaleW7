using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Web;

namespace ProgettoSettimanaleW7.Models
{
    public class Articoli
    {
        [Key]
        public int IdArticolo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(200)]
        public string Immagine { get; set; }

        [Required]
        public decimal Prezzo { get; set; }

        [Required]
        public int TempoConsegna { get; set; }
    }
}