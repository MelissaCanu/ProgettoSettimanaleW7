using ProgettoSettimanaleW7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProgettoSettimanaleW7.ViewModel
{
    public class OrderEditViewModel
    {
        public Ordini Order { get; set; }
        public IEnumerable<SelectListItem> ArticoliList { get; set; }
        public SelectList IdUtenteList { get; internal set; }
    }
}
