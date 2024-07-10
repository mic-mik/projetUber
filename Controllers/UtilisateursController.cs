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
    public class UtilisateursController : Controller
    {
        private GoFastDb db = new GoFastDb();

        // GET: Utilisateurs
        public ActionResult Index()
        {
            var utilisateur = db.Utilisateur.Include(u => u.Abonnement).Include(u => u.Adresse);
            return View(utilisateur.ToList());
        }

        // GET: Utilisateurs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public ActionResult Create()
        {
            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue");
            return View();
        }

        // POST: Utilisateurs/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UtilisateurId,Nom,Prenom,Email,Telephone,MotDePasse,Roles,AdresseId,AbonnementId,estPremiereConnection,ImagePath")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                db.Utilisateur.Add(utilisateur);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre", utilisateur.AbonnementId);
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue", utilisateur.AdresseId);
            return View(utilisateur);
        }

        // GET: Utilisateurs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre", utilisateur.AbonnementId);
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue", utilisateur.AdresseId);
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UtilisateurId,Nom,Prenom,Email,Telephone,MotDePasse,Roles,AdresseId,AbonnementId,estPremiereConnection,ImagePath")] Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                db.Entry(utilisateur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre", utilisateur.AbonnementId);
            ViewBag.AdresseId = new SelectList(db.Adresse, "AdresseId", "Rue", utilisateur.AdresseId);
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            db.Utilisateur.Remove(utilisateur);
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
