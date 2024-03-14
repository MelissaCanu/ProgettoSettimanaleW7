using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProgettoSettimanaleW7.Models;

namespace ProgettoSettimanaleW7.Controllers
{
    public class OrdiniController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Ordini
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var ordini = db.Ordini.Include(o => o.Utenti);
            return View(ordini.ToList());
        }

        // GET: Ordini/Details/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // GET: Ordini/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username");
            return View();
        }

        // POST: Ordini/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "IdOrdine,IdUtente,Indirizzo,IsEvaso,Note")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Ordini.Add(ordini);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username", ordini.IdUtente);
            return View(ordini);
        }

        // GET: Ordini/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username", ordini.IdUtente);
            return View(ordini);
        }

        // POST: Ordini/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "IdOrdine,IdUtente,Indirizzo,IsEvaso,Note")] Ordini ordini)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordini).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username", ordini.IdUtente);
            return View(ordini);
        }

        // GET: Ordini/Delete/5
        [Authorize(Roles = "User, Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordini ordini = db.Ordini.Find(id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
        }

        // POST: Ordini/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User, Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordini ordini = db.Ordini.Find(id);
            db.Ordini.Remove(ordini);
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

        //CARRELLO - CHECKOUT
        [Authorize(Roles = "User")]
        public ActionResult Checkout()
        {
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Ordini.Include(o => o.DettagliOrdini.Select(d => d.Articoli)).SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        //CARRELLO - COMPLETA ORDINE

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult CompleteOrder(int id, string shippingAddress, string notes)
        {
            var order = db.Ordini.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // Aggiungi la validazione dell'indirizzo qui
            if (string.IsNullOrEmpty(shippingAddress))
            {
                ModelState.AddModelError("shippingAddress", "L'indirizzo di spedizione è obbligatorio.");
            }
            else
            {
                order.Indirizzo = shippingAddress;
            }

            order.Note = notes;
            order.IsEvaso = true;

            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Se la validazione fallisce, ritorna alla vista con l'ordine
            return View(order);
        }

        //CARRELLO - RIMUOVI DAL CARRELLO

        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult RemoveFromCart(int productId)
        {
            // Recupero l'utente corrente
            var user = db.Utenti.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Utente non trovato");
            }

            // Cerco un ordine esistente non evaso per l'utente corrente
            var order = db.Ordini.SingleOrDefault(o => o.IdUtente == user.IdUtente && !o.IsEvaso);
            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ordine non trovato");
            }

            // Cerco un DettagliOrdini esistente per l'articolo specificato
            var orderDetail = order.DettagliOrdini.FirstOrDefault(d => d.Articoli.IdArticolo == productId);
            if (orderDetail == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Articolo non trovato nell'ordine");
            }

            // Rimuovo l'articolo dall'ordine
            order.DettagliOrdini.Remove(orderDetail);
            db.DettagliOrdini.Remove(orderDetail);

            try
            {
                db.SaveChanges();
                TempData["SuccessMessage"] = "Prodotto rimosso dal carrello con successo!";
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

            return RedirectToAction("Checkout");
        }





    }
}
