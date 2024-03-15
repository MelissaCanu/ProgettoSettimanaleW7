using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ProgettoSettimanaleW7.Models;
using ProgettoSettimanaleW7.ViewModel;

namespace ProgettoSettimanaleW7.Controllers
{
    public class OrdiniController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Ordini/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.Articoli = new SelectList(db.Articoli, "IdArticolo", "Nome");

            // Add this line to populate the ViewData for 'IdUtente'
            ViewData["IdUtente"] = new SelectList(db.Utenti, "IdUtente", "Username");

            return View();
        }

        // POST: Ordini/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Ordini ordini, int IdArticolo, int Quantita)
        {
            if (ModelState.IsValid)
            {
                //aggiungo l'ordine al db
                db.Ordini.Add(ordini);
                db.SaveChanges();
                //imposto la data dell'ordine e lo stato di evasione
                ordini.DataOrdine = DateTime.Now;
                ordini.IsEvaso = false;


                // prendo l'articolo selezionato e calcolo il prezzo totale
                var articolo = db.Articoli.Find(IdArticolo);
                if (articolo == null)
                {
                    return HttpNotFound();
                }
 
                var totalPrice = articolo.Prezzo * Quantita;

                // creo un nuovo dettaglio ordine e lo aggiungo al db
                var dettagliOrdini = new DettagliOrdini
                {
                    IdOrdine = ordini.IdOrdine,
                    IdArticolo = IdArticolo,
                    Quantita = Quantita,
                    PrezzoTotale = totalPrice
                };

                db.DettagliOrdini.Add(dettagliOrdini);
                db.SaveChanges();

                return RedirectToAction("ManageOrders");
            }

            //questo mi serve per popolare la dropdownlist con gli utenti esistenti
            ViewBag.IdUtente = new SelectList(db.Utenti, "IdUtente", "Username", ordini.IdUtente);
            return View(ordini);
        }


        // GET: Ordini/Delete/5
        [Authorize(Roles = "Admin")]
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
            return RedirectToAction("ManageOrders");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Includo utenti e dettagli ordini quando recupero l'ordine così da poter visualizzare i dettagli 
            Ordini ordini = db.Ordini.Include(o => o.Utenti).Include(o => o.DettagliOrdini.Select(d => d.Articoli)).SingleOrDefault(o => o.IdOrdine == id);
            if (ordini == null)
            {
                return HttpNotFound();
            }
            return View(ordini);
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

            // Aggiungo la validazione dell'indirizzo qui
            if (string.IsNullOrEmpty(shippingAddress))
            {
                ModelState.AddModelError("shippingAddress", "L'indirizzo di spedizione è obbligatorio.");
            }
            else
            {
                order.Indirizzo = shippingAddress;
            }

            order.Note = notes;

            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("UserDetails", new { id = order.IdOrdine });
            }

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

        // CARRELLO - DETTAGLI ORDINE PER USER 
        [Authorize(Roles = "User")]

        //recupero l'user loggato e i suoi ordini non evasi
        public ActionResult UserDetails(int? id)
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

        //ORDINI - GESTIONE & EVASIONE - ADMIN

        [Authorize(Roles = "Admin")]
        public ActionResult ManageOrders()
        {
            var orders = db.Ordini.ToList();
            return View(orders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult MarkAsEvaso(int id)
        {
            var order = db.Ordini.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            //se l'ordine non è evaso, lo evado e salvo le modifiche nel db
            order.IsEvaso = true;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ManageOrders");
        }

        //********************************************************************************************************************
        //CHIAMATE ASYNC PER QUERY AL DB 

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetTotalOrders()
        {
            //recupero il numero totale di ordini evasi e lo ritorno come JSON 
            var totalOrders = await db.Ordini.CountAsync(o => o.IsEvaso);
            return Json(totalOrders, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetTotalRevenue(DateTime date)
        {
            //recupero il totale delle entrate per una data specifica e lo ritorno come JSON
            var totalRevenue = await db.Ordini
                .Where(o => DbFunctions.TruncateTime(o.DataOrdine) == date.Date && o.IsEvaso)
                .SumAsync(o => o.DettagliOrdini.Sum(d => d.PrezzoTotale));
            return Json(totalRevenue, JsonRequestBehavior.AllowGet);

        }
    }
}
