using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProgettoSettimanaleW7.Models
{
    public class Ordini
    {
        [Key]
        public int IdOrdine { get; set; }

        [ForeignKey("Utenti")]
        public int IdUtente { get; set; }

        [StringLength(100)]
        public string Indirizzo { get; set; }

        [Display(Name = "Evaso")]
        public bool IsEvaso { get; set; }

        [StringLength(200)]
        public string Note { get; set; }

        //relazione con la tabella Utenti e DettagliOrdini 
        public virtual Utenti Utenti { get; set; }

        //ICollection perché un ordine può avere più dettagli
        public virtual ICollection<DettagliOrdini> DettagliOrdini { get; set; }


    }
}