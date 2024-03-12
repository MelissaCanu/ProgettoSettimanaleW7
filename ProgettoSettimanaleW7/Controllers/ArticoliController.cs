using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProgettoSettimanaleW7.Models;

namespace ProgettoSettimanaleW7.Controllers
{
    public class ArticoliController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Articoli
        public ActionResult Index()
        {
            return View(db.Articoli.ToList());
        }

        // GET: Articoli/Details/5
        public ActionResult Details(int? id)
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

        // GET: Articoli/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articoli/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}
