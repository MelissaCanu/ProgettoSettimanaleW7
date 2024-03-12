using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProgettoSettimanaleW7.Models;

namespace ProgettoSettimanaleW7.Controllers
{
    public class UtentiController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Utenti
        public ActionResult Index()
        {
            return View(db.Utenti.ToList());
        }

        // GET: Utenti/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            return View(utenti);
        }

        // GET: Utenti/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Utenti/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUtente,Username,Password,Ruolo")] Utenti utenti)
        {
            if (ModelState.IsValid)
            {
                db.Utenti.Add(utenti);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(utenti);
        }

        // GET: Utenti/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            return View(utenti);
        }

        // POST: Utenti/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUtente,Username,Password,Ruolo")] Utenti utenti)
        {
            if (ModelState.IsValid)
            {
                db.Entry(utenti).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(utenti);
        }

        // GET: Utenti/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utenti utenti = db.Utenti.Find(id);
            if (utenti == null)
            {
                return HttpNotFound();
            }
            return View(utenti);
        }

        // POST: Utenti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utenti utenti = db.Utenti.Find(id);
            db.Utenti.Remove(utenti);
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

        //Login 

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Utenti utenti)
        {
            // Cerco l'utente nel database in base allo username inserito dall'utente
            var userInDb = db.Utenti.FirstOrDefault(u => u.Username == utenti.Username);

            if (userInDb != null)
            {
                // Verifico la password inserita dall'utente con quella presente nel database
                if (userInDb.Password == utenti.Password)
                {
                    FormsAuthentication.SetAuthCookie(utenti.Username, false);
                    RoleProvider roleProvider = Roles.Provider;
                    string[] roles = roleProvider.GetRolesForUser(utenti.Username);
                    if (roles.Contains("Admin"))
                    {
                        TempData["SuccessMessage"] = "Login effettuato con successo!";

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Login effettuato con successo!";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Password non valida");
                }
            }
            else
            {
                ModelState.AddModelError("", "Username non valido");
            }

            return View(utenti);
        }

        //Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //Registrazione
        public ActionResult Registrazione()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrazione([Bind(Include = "IdUtente,Username,Password")] Utenti utenti)
        {
            if (ModelState.IsValid)
            {
                //Verifico se esiste già user con stesso username
                var existingUser = db.Utenti.FirstOrDefault(u => u.Username == utenti.Username);
                if (existingUser != null)
                { 
                    ModelState.AddModelError("", "Username già esistente");
                    return View(utenti);
                }
                //Assegno il ruolo default "User" ai nuovi utenti
                utenti.Ruolo = "User";

                //Aggiungo l'utente al database e salvo le modifiche
                db.Utenti.Add(utenti);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Registrazione effettuata con successo!";
                return RedirectToAction("Index", "Home");
            }
            TempData["ErrorMessage"] = "Si è verificato un errore durante la registrazione.";
            return View(utenti);
        }

    }
}
