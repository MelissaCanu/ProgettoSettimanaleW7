using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProgettoSettimanaleW7.Models;

namespace ProgettoSettimanaleW7.Controllers
{
    
    public class ArticoliController : Controller
        
        //modeldbcontext mi permette di accedere al database e di eseguire le operazioni CRUD
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Articoli
        [AllowAnonymous]
        public ActionResult Index()
        {   
            //Controllo se l'utente è admin
            var isUserAdmin = User.IsInRole("Admin");
            //Passo alla view la variabile isUserAdmin
            //che mi permette di visualizzare o meno i pulsanti di modifica e cancellazione
            ViewBag.isUserAdmin = isUserAdmin;
            return View(db.Articoli.ToList());
        }

        // GET: Articoli/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Find mi permette di trovare un articolo in base all'id
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            //Passo alla view la variabile isUserAdmin per visualizzare o meno i pulsanti di modifica e cancellazione
            ViewBag.IsUserAdmin = User.IsInRole("Admin");
            return View(articoli);
        }

        // GET: Articoli/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articoli/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Articoli articoli, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img/"), fileName);
                    file.SaveAs(path);
                    articoli.Immagine = fileName;  // Salva il nome del file nel campo Immagine
                }

                db.Articoli.Add(articoli);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(articoli);
        }


        // GET: Articoli/Edit/5
        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int? id)

        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int id, Articoli articoli, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img/"), fileName);
                    file.SaveAs(path);
                    articoli.Immagine = fileName;  // Salva il nome del file nel campo Immagine
                }

                db.Entry(articoli).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(articoli);
        }

        // GET: Articoli/Delete/5
        [Authorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articoli articoli = db.Articoli.Find(id);
            if (articoli == null)
            {
                return HttpNotFound();
            }
            return View(articoli);
        }

        // POST: Articoli/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult DeleteConfirmed(int id)
        {
            Articoli articoli = db.Articoli.Find(id);
            db.Articoli.Remove(articoli);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //CARRELLO

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult AddToCart(int productId, int quantity)
        {
            // Recupero l'utente corrente
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente non trovato");
            }

            // Cerco un ordine esistente non evaso per l'utente corrente
            var order = db.Ordini.SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);

            // Se non esiste un ordine, ne creo uno nuovo
            if (order == null)
                {
                    order = new Ordini { IdUtente = user.IdUtente, IsEvaso = false, DettagliOrdini = new List<DettagliOrdini>(), DataOrdine = DateTime.Now };
                    db.Ordini.Add(order);
                }

            // Cerco un DettagliOrdini esistente per l'articolo specificato
            var orderDetail = order.DettagliOrdini.FirstOrDefault(d => d.Articoli.IdArticolo == productId);

            // Se esiste, incremento la quantità - altrimenti creo un nuovo DettagliOrdini
            if (orderDetail != null)
            {
                orderDetail.Quantita += quantity;
                orderDetail.PrezzoTotale = orderDetail.Articoli.Prezzo * orderDetail.Quantita;
            }
            else
            {
                // Aggiungo il prodotto all'ordine
                var product = db.Articoli.Find(productId);
                if (product == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Prodotto non trovato");
                }
                order.DettagliOrdini.Add(new DettagliOrdini { Articoli = product, Quantita = quantity, PrezzoTotale = product.Prezzo * quantity });
            }

            try
            {
                db.SaveChanges();
                TempData["SuccessMessage"] = "Prodotto aggiunto al carrello con successo!";
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                TempData["ErrorMessage"] = "Errore durante il salvataggio dei dati";
            }

             return RedirectToAction("Index");
        }

       









    }
}
