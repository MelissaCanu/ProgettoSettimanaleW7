using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProgettoSettimanaleW7.Models
{
    public class DettagliOrdini
    {
        [Key]
        public int IdDettagliOrdine { get; set; }

        [ForeignKey("Ordini")]
        public int IdOrdine { get; set; }

        [ForeignKey("Articoli")]
        public int IdArticolo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere almeno 1")]
        public int Quantita { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il prezzo totale deve essere almeno 0.01")]
        public decimal PrezzoTotale { get; set; }

        public virtual Ordini Ordini { get; set; }
        public virtual Articoli Articoli { get; set; }
    }
}