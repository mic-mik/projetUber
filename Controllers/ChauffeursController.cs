using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Projet_GoFast.Models;

namespace Projet_GoFast.Controllers
{
    public class ChauffeursController : Controller
    {
        private GoFastDb db = new GoFastDb();

        // GET: Chauffeurs
        public ActionResult Index()
        {
            var chauffeur = db.Chauffeur.Include(c => c.Adresse).Include(c => c.Voiture);
            return View(chauffeur.ToList());
        }

        // GET: Chauffeurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            if (chauffeur == null)
            {
                return HttpNotFound();
            }
            return View(chauffeur);
        }

        // GET: Chauffeurs/Create
        public ActionResult Create()
        {
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue");
            ViewBag.VoitureId = new SelectList(db.Voiture, "VoitureId", "Modele");
            return View();
        }

        // POST: Chauffeurs/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ChauffeurId,Nom,Prenom,Email,Telephone,MotDePasse,Roles,estPremiereConnection,VoitureId,AdresseId")] Chauffeur chauffeur)
        {
            if (ModelState.IsValid)
            {
                db.Chauffeur.Add(chauffeur);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue", chauffeur.AdresseId);
            ViewBag.VoitureId = new SelectList(db.Voiture, "VoitureId", "Modele", chauffeur.VoitureId);
            return View(chauffeur);
        }

        // GET: Chauffeurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            if (chauffeur == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue", chauffeur.AdresseId);
            ViewBag.VoitureId = new SelectList(db.Voiture, "VoitureId", "Modele", chauffeur.VoitureId);
            return View(chauffeur);
        }

        // POST: Chauffeurs/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ChauffeurId,Nom,Prenom,Email,Telephone,MotDePasse,Roles,estPremiereConnection,VoitureId,AdresseId")] Chauffeur chauffeur)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chauffeur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue", chauffeur.AdresseId);
            ViewBag.VoitureId = new SelectList(db.Voiture, "VoitureId", "Modele", chauffeur.VoitureId);
            return View(chauffeur);
        }

        // GET: Chauffeurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            if (chauffeur == null)
            {
                return HttpNotFound();
            }
            return View(chauffeur);
        }

        // POST: Chauffeurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            db.Chauffeur.Remove(chauffeur);
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
