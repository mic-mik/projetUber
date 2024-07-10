using Projet_GoFast.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace Projet_GoFast.Controllers
{
    public class DriversController : Controller
    {
        // GET: Drivers
        private GoFastDb db = new GoFastDb();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inscription()
        {
            var Province = db.Province.ToList();
            ViewBag.ProvinceId = new SelectList(Province, "ProvinceId", "Symbole");

            var marques = db.Marque.ToList();  
            ViewBag.MarqueId = new SelectList(marques, "MarqueId", "Titre");
            return View(new Chauffeur());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscription(Chauffeur chauffeur)
        {
            if (!string.IsNullOrEmpty(chauffeur.Nom) &&
                !string.IsNullOrEmpty(chauffeur.Prenom) && !string.IsNullOrEmpty(chauffeur.Email) &&
                !string.IsNullOrEmpty(chauffeur.Telephone) && chauffeur.Voiture != null && chauffeur.Adresse != null
                && chauffeur.ImageFile != null && chauffeur.ImageFile.ContentLength > 0)
            {
                // to do : tester si Email est valide 
                if (ValiderMail(chauffeur) == true)
                {
                    if (ValiderNom(chauffeur) == true)
                    {

                        if (ValiderNumero(chauffeur) == true)
                        {
                            if (ValiderPrenom(chauffeur) == true)
                            {

                                //to do : tester si Mail existe 
                                int nb = (db.Chauffeur.Where(x => x.Email == chauffeur.Email)).Count();
                                if (nb > 0) //chauffeur existe deja 
                                {
                                    ViewBag.Message = "Mail existe déja ";
                                    ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                                    ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                                    return View(chauffeur);
                                }
                                else
                                {
                                    // To do : Crypter le mot de passe
                                    string MotPasse = GenererMotPasse(chauffeur);
                                    string hashedPassword = Crypto.HashPassword(MotPasse);
                                    db.Voiture.Add(chauffeur.Voiture);
                                    db.SaveChanges();

                                    db.Adresse.Add(chauffeur.Adresse);
                                    db.SaveChanges();

                                    var fileName = Path.GetFileNameWithoutExtension(chauffeur.ImageFile.FileName);
                                    var extension = Path.GetExtension(chauffeur.ImageFile.FileName);
                                    var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                                    var path = Path.Combine(Server.MapPath("~/Photos/"), uniqueFileName);
                                    chauffeur.ImageFile.SaveAs(path);

                                    Chauffeur newChauffeur = new Chauffeur
                                    {
                                        Nom = chauffeur.Nom.Trim(),
                                        Prenom = chauffeur.Prenom.Trim(),
                                        Email = chauffeur.Email.Trim(),
                                        MotDePasse = hashedPassword,
                                        Telephone = chauffeur.Telephone?.Trim(),
                                        Roles = 1,
                                        estPremiereConnection = 0,
                                        VoitureId = chauffeur.Voiture.VoitureId,
                                        AdresseId = chauffeur.Adresse.AdresseId,
                                        ImagePath = uniqueFileName,
                                    };

                                    db.Chauffeur.Add(newChauffeur);
                                    db.SaveChanges();

                                    //To do : Envoyé email 
                                    if (SendMail(chauffeur) == true)
                                    {
                                        ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                                        ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                                        ViewBag.Message = "Verifier votre courriel, un mail a été envoyé !";
                                        return View(chauffeur);
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Error lors de l'envoie du message";
                                        ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                                        ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                                        return View(new Chauffeur());
                                    }

                                }


                            }
                            ViewBag.Message = " Erreur ! le prenom doit contenir que des lettres ";
                            ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                            ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                            return View(new Chauffeur());


                        }


                        ViewBag.Message = "Le Telephone doit contenir uniquement des chiffres et être composé de 9 chiffres maximum.";
                        ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                        ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                        return View(new Chauffeur());


                    }
                    ViewBag.Message = " Erreur ! le nom doit contenir que des lettres ";
                    ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                    ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                    return View(new Chauffeur());


                }
                else
                {
                    ViewBag.Message = " Erreur ! le format de votre Email n'est pas valide  ";
                    ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                    ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                    return View(new Chauffeur());
                }
            }
            var provinces = db.Province.ToList();
            ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "Symbole");

            var marques = db.Marque.ToList();
            ViewBag.MarqueId = new SelectList(marques, "MarqueId", "Titre");
            ViewBag.Message = "Veuillez remplir tous les champs requis.";

            return View(new Chauffeur());


        }

        private string GenererMotPasse(Chauffeur ChauffeurInfo)
        {
            string nom = ChauffeurInfo.Nom;
            string prenom = ChauffeurInfo.Prenom;
            string lettresPairesNoom = "";
            for (int i = 1; i < nom.Length; i += 2)
            {
                lettresPairesNoom += nom[i];
            }

            string lettresImpairesPrenom = "";
            for (int i = 0; i < prenom.Length; i += 2)
            {
                lettresImpairesPrenom += prenom[i];
            }

            int moisActuelle = DateTime.Now.Month;
            int dayActuelle = DateTime.Now.Day;
            string password = lettresPairesNoom + dayActuelle.ToString() + lettresImpairesPrenom + moisActuelle.ToString();
            return password;
        }


        private bool EstValide(string partie, string pattern)
        {
            return Regex.IsMatch(partie, pattern);
        }

        private bool ValiderMail(Chauffeur ChauffeurInfo)
        {
            if (ChauffeurInfo.Email.Contains("@") && ChauffeurInfo.Email.Contains("."))
            {
                string[] parties = ChauffeurInfo.Email.Split('@', '.');

                if (parties.Length == 3)
                {
                    string partie1 = parties[0];
                    string partie2 = parties[1];
                    string partie3 = parties[2];

                    if (EstValide(partie1, @"^[a-zA-Z0-9._-]+$") &&
                        EstValide(partie2, @"^[a-zA-Z_-]+$") &&
                        EstValide(partie3, "^[a-zA-Z]+$"))
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        private bool SendMailOublier(Chauffeur ChauffeurInfo)
        {
            string motpass = Session["motPasse"].ToString();
            string suject = "Bienvenue sur GoFast";
            string Message = $"Bienvenue sur GoFast\n" +
                $"Voici les informations pour la réeintialisation du mot de passe :\nMot de passe  : {motpass}\n" +
                $"Utiliser cette information pour vous conncetez \n Cordialement.";

            string sender = "LoicSagbo@teccart.online";
            string pw = "Tecc12123";

            try
            {
                SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587);
                smtpclient.Timeout = 3000;
                smtpclient.EnableSsl = true;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new NetworkCredential(sender, pw);
                MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, ChauffeurInfo.Email, suject, Message);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                smtpclient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }

        }



        private bool SendMail(Chauffeur ChauffeurInfo)
        {

            string suject = "Bienvenue sur GoFast";
            string Message = $"Bienvenue sur GoFast\n" +
                $"Voici les informations pour votre première connexion:\nEmail : {ChauffeurInfo.Email}\nMot de passe : {GenererMotPasse(ChauffeurInfo)}" +
                $"\nUtiliser cette information pour vous conncetez " +
                $"\n Cordialement."+

            $"\nCordialement.";

            string sender = "LoicSagbo@teccart.online";
            string pw = "Tecc12123";

            try
            {
                SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587);
                smtpclient.Timeout = 3000;
                smtpclient.EnableSsl = true;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new NetworkCredential(sender, pw);
                MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, ChauffeurInfo.Email, suject, Message);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                smtpclient.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return false;
            }

        }
        private bool ValiderNom(Chauffeur ChauffeurInfo)
        {
            if (string.IsNullOrEmpty(ChauffeurInfo.Nom) || Regex.IsMatch(ChauffeurInfo.Nom, @"^[a-zA-Z]+$"))
            {
                return true;
            }
            return false;
        }

        private bool ValiderPrenom(Chauffeur ChauffeurInfo)
        {
            if (string.IsNullOrEmpty(ChauffeurInfo.Prenom) || Regex.IsMatch(ChauffeurInfo.Prenom, @"^[a-zA-Z]+$"))
            {
                return true;
            }
            return false;
        }



        private bool ValiderNumero(Chauffeur ChauffeurInfo)
        {
            if (string.IsNullOrEmpty(ChauffeurInfo.Telephone) || Regex.IsMatch(ChauffeurInfo.Telephone, @"^\d{1,9}$"))
            {
                return true;
            }
            return false;
        }

        public ActionResult Connexion()
        {
            return View(new Chauffeur());
        }

        [HttpPost]
        public ActionResult Connexion(Chauffeur ChauffeurInfo)
        {
            // Vérifiez que les champs ne sont pas null ou vides
            if (string.IsNullOrEmpty(ChauffeurInfo.MotDePasse) || string.IsNullOrEmpty(ChauffeurInfo.Email))
            {
                ViewBag.Message = "Erreur ! Veuillez entrer tous les champs.";
                return View();
            }
            string Email = ChauffeurInfo.Email.Trim();


            Chauffeur chauffeur = db.Chauffeur.SingleOrDefault(m => m.Email == Email);

            // Vérifiez si le chauffeur existe
            if (chauffeur == null)
            {
                ViewBag.Message = "Erreur ! Informations invalides, veuillez réessayer.";
                return View();
            }
            else
            {
                string hashedPassword = chauffeur.MotDePasse;
                string passwordToCheck = ChauffeurInfo.MotDePasse;

                // Vérifier le mot de passe
                bool passwordMatch = Crypto.VerifyHashedPassword(hashedPassword, passwordToCheck);

                if (passwordMatch)
                {
                    if (chauffeur.estPremiereConnection != 0)
                    {
                        Session["ChauffeurID"] = chauffeur.ChauffeurId;
                        return RedirectToAction("Acceuil");
                    }
                    else
                    {
                        Session["ChauffeurID"] = chauffeur.ChauffeurId;
                        return RedirectToAction("PremiereConnexion");
                    }
                }
                else if (ChauffeurInfo.MotDePasse.Trim() == chauffeur.MotDePasse)
                {
                    Session["ChauffeurID"] = chauffeur.ChauffeurId;
                    return RedirectToAction("PremiereConnexion");

                }
                else
                {
                    ViewBag.Message = "Erreur ! Informations invalides, veuillez réessayer.";
                    return View();
                }

            }



        }


        public ActionResult Acceuil()
        {
            int id = Convert.ToInt32(Session["ChauffeurID"]);
            return View();
        }

        public ActionResult PremiereConnexion()
        {
            int id = Convert.ToInt32(Session["ChauffeurID"]);
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            if (chauffeur == null)
            {
                return HttpNotFound();
            }
            return View(new Chauffeur());
        }

        [HttpPost]
        public ActionResult PremiereConnexion(Chauffeur chauffeur)
        {
            int id = Convert.ToInt32(Session["ChauffeurID"]);
            Chauffeur chauffeurs = db.Chauffeur.Find(id);
            if (chauffeur.MotDePasse != null)
            {
                if (chauffeurs == null)
                {
                    return HttpNotFound();
                }

                // Mettez à jour uniquement le mot de passe
                // To do : Crypter le mot de passe
                string hashedPassword = Crypto.HashPassword(chauffeur.MotDePasse);
                chauffeurs.MotDePasse = hashedPassword;
                chauffeurs.estPremiereConnection = 1;
                db.SaveChanges();
                return RedirectToAction("Connexion");
            }
            return RedirectToAction("Connexion");
        }

        public ActionResult Deconnexion()
        {
            Session.Clear();
            return RedirectToAction("Connexion", "Drivers");
        }

        public ActionResult Profil()
        {
            int id = Convert.ToInt32(Session["ChauffeurID"]);
            if (id == 0)
            {
                return RedirectToAction("Acceuil", "Drivers");
            }
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            if (chauffeur == null)
            {
                return RedirectToAction("Acceuil", "Drivers");
            }
            ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
            ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
            return View(chauffeur);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profil( Chauffeur chauffeur)
        {
            int id = Convert.ToInt32(Session["ChauffeurID"]);
            if (ModelState.IsValid)
            {
                db.Entry(chauffeur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Acceuil", "Drivers");
            }
            ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
            ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
            return View(chauffeur);
        }

        public ActionResult MotPaaseOublie()
        {

            return View(new Chauffeur());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MotPaaseOublie(Chauffeur chauffeur)
        {
            if (!string.IsNullOrEmpty(chauffeur.Email))
            {
                Chauffeur chaff= db.Chauffeur.SingleOrDefault(x => x.Email == chauffeur.Email);

                if (chaff != null) //chauffeur existe deja 
                {
                    string motPasse = chaff.MotDePasse;
                    Session["motPasse"] = motPasse;

                    //To do : Envoyé email 
                    if (SendMailOublier(chauffeur) == true)
                    {
                        ViewBag.Message = "Verifier votre courriel, un mail a été envoyé !";
                        return View(chauffeur);    
                    }
                    else
                    {
                        ViewBag.Message = "Error lors de l'envoie du message";
                        return View(chauffeur);
                    }
                
                }
                else
                {

                    ViewBag.Message = "Email introuvable, veuillez réessayer !";
                    return View();

                }
            }
            ViewBag.Message = "Veuillez remplir tous les champs requis.";
            return View(new Chauffeur());

        }


        public ActionResult Voir_Profil()
        {
            int id = Convert.ToInt32(Session["ChauffeurID"]);
            var chauffeur = db.Chauffeur
                                .Include(u => u.Voiture)
                                .Include(u => u.Adresse)
                                .Include(u => u.Adresse.Province)
                                .FirstOrDefault(m => m.ChauffeurId == id);


            return View(chauffeur);
        }
        

        public ActionResult DeleteProfil(int id)
        {
            Adresse Adresse = db.Adresse.Find(id);
            Chauffeur chauffeur = db.Chauffeur.Find(id);
            Voiture Voiture = db.Voiture.Find(id);
            db.Chauffeur.Remove(chauffeur);
            db.Voiture.Remove(Voiture);
            db.Adresse.Remove(Adresse);
            db.SaveChanges();
            return RedirectToAction("Connexion");
        }


        public ActionResult EditProfil(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Voir_Profil");
            }else
       
            {
                Chauffeur chauffeur = db.Chauffeur.Find(id);
                if (chauffeur == null)
                {
                    return RedirectToAction("Voir_Profil");
                }
                else
                {
                    if (!string.IsNullOrEmpty(chauffeur.Nom) &&
                     !string.IsNullOrEmpty(chauffeur.Prenom) && !string.IsNullOrEmpty(chauffeur.Email) &&
                     !string.IsNullOrEmpty(chauffeur.Telephone) && chauffeur.Voiture != null && chauffeur.Adresse != null
                     && chauffeur.ImageFile != null && chauffeur.ImageFile.ContentLength > 0)
                    {
                        ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                        ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                        return View(chauffeur);

                    }
                    var provinces = db.Province.ToList();
                    ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "Symbole");

                    var marques = db.Marque.ToList();
                    ViewBag.MarqueId = new SelectList(marques, "MarqueId", "Titre");
  
                    return View(new Chauffeur());

                }

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfil(Chauffeur chauffeur)
        {
            if (!ModelState.IsValid)
            {
                var provinces = db.Province.ToList();
                ViewBag.ProvinceId = new SelectList(provinces, "ProvinceId", "Symbole");

                var marques = db.Marque.ToList();
                ViewBag.MarqueId = new SelectList(marques, "MarqueId", "Titre");
                ViewBag.Message = "Veuillez remplir tous les champs requis.";

                return View(chauffeur);
            }

            // Valider l'e-mail
            if (!ValiderMail(chauffeur))
            {
                ViewBag.Message = "Erreur ! Le format de votre e-mail n'est pas valide.";
                ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");
                ViewBag.MarqueId = new SelectList(db.Marque.ToList(), "MarqueId", "Titre");
                return View(chauffeur);
            }

            try
            {
                var existingChauffeur = db.Chauffeur.Include(c => c.Voiture).Include(c => c.Adresse).SingleOrDefault(c => c.ChauffeurId == chauffeur.ChauffeurId);

                if (existingChauffeur != null)
                {
                    // Mettre à jour seulement les champs modifiés
                    existingChauffeur.Nom = chauffeur.Nom.Trim();
                    existingChauffeur.Prenom = chauffeur.Prenom.Trim();
                    existingChauffeur.Email = chauffeur.Email.Trim();
                    existingChauffeur.Telephone = chauffeur.Telephone?.Trim();

                    if (chauffeur.Voiture != null)
                    {
                        // Mettre à jour les détails du véhicule uniquement si un nouveau véhicule est sélectionné
                        existingChauffeur.Voiture.Marque = chauffeur.Voiture.Marque;
                        existingChauffeur.Voiture.Modele = chauffeur.Voiture.Modele;
                        existingChauffeur.Voiture.Annee = chauffeur.Voiture.Annee;
                    }

                    if (chauffeur.Adresse != null)
                    {
                        // Mettre à jour les détails d'adresse uniquement si une nouvelle adresse est entrée
                        existingChauffeur.Adresse.Ville = chauffeur.Adresse.Ville;
                        existingChauffeur.Adresse.Rue = chauffeur.Adresse.Rue;
                        existingChauffeur.Adresse.CodePostal = chauffeur.Adresse.CodePostal;
                    }

                    // Sauvegarder les modifications
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Error = "Chauffeur non trouvé.";
                    return View(chauffeur);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Une erreur s'est produite lors de la sauvegarde des données : " + ex.Message;
                return View(chauffeur);
            }

            return RedirectToAction("Index");
        }






    }
}