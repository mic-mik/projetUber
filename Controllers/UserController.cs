using Projet_GoFast.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Reflection.Emit;
using System.Web.UI.WebControls;
using System.Web.Helpers;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;

namespace Projet_GoFast.Controllers
{
    public class UserController : Controller
    {
        //Instancedb
        private GoFastDb db = new GoFastDb();

        // GET: User

        public ActionResult Index()
        {
            var optionsINSCRIRE = new List<SelectListItem>
            {   
                new SelectListItem { Text = "S'INSCRIRE", Value = "", Selected = true },
                new SelectListItem { Text = "Chauffeur", Value = "Chauffeur" },
                new SelectListItem { Text = "Passager", Value = "Passager" },
            };

            ViewBag.INSCRIRE = optionsINSCRIRE;

            var optionsCon = new List<SelectListItem>
            {
                new SelectListItem { Text = "SE CONNECTER", Value = "", Selected = true },
                new SelectListItem { Text = "Chauffeur", Value = "Chauffeur" },
                new SelectListItem { Text = "Passager", Value = "Passager" },
            };

            ViewBag.CONNECTER = optionsCon;


            return View(new CommentairePublic());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( CommentairePublic commentairePublic)
        {
            if (commentairePublic.Nom != null && commentairePublic.Prenom != null && commentairePublic.Commentaire != null)
            {
                db.CommentairePublic.Add(commentairePublic);
                db.SaveChanges();
                ViewBag.Message1 = "Commentaire envoyé avec succès ! ";     
                return View("Index");
            }
            ViewBag.Message1 = "Veuillez remplir tous les champs !";
            return View(commentairePublic);
            
        }



        //Inscription
        public ActionResult Inscription()
        {
            ViewBag.AbonnementId = new SelectList(db.Abonnement.ToList(), "AbonnementId", "Titre");
            ViewBag.ProvinceId = new SelectList(db.Province.ToList(), "ProvinceId", "Symbole");

            return View(new Utilisateur());
        }
        [HttpPost]
     
        public ActionResult Inscription(Utilisateur utilisateurInfo)
        {
            if (!string.IsNullOrEmpty(utilisateurInfo.Nom) &&
                !string.IsNullOrEmpty(utilisateurInfo.Prenom) && !string.IsNullOrEmpty(utilisateurInfo.Email) &&
                !string.IsNullOrEmpty(utilisateurInfo.Telephone) && utilisateurInfo.Abonnement != null && utilisateurInfo.Adresse != null && utilisateurInfo.ImageFile != null && utilisateurInfo.ImageFile.ContentLength > 0)
            {
                if (ValiderMail(utilisateurInfo))
                {
                    int nb = db.Utilisateur.Count(x => x.Email == utilisateurInfo.Email);
                    if (nb > 0)
                    {
                        ViewBag.Message = "Mail existe déjà";
                        ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
                        ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole");
                        return View(utilisateurInfo);
                    }
                    else
                    {
                        string MotPasse = GenererMotPasse(utilisateurInfo);
                        string hashedPassword = Crypto.HashPassword(MotPasse);

                        db.Adresse.AddOrUpdate(utilisateurInfo.Adresse);
                        db.SaveChanges();

                        var fileName = Path.GetFileNameWithoutExtension(utilisateurInfo.ImageFile.FileName);
                        var extension = Path.GetExtension(utilisateurInfo.ImageFile.FileName);
                        var uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{extension}";
                        var path = Path.Combine(Server.MapPath("~/Photos/"), uniqueFileName);
                        utilisateurInfo.ImageFile.SaveAs(path);

                        Utilisateur NewUtilisateur = new Utilisateur
                        {
                            Nom = utilisateurInfo.Nom.Trim(),
                            Prenom = utilisateurInfo.Prenom.Trim(),
                            Email = utilisateurInfo.Email.Trim(),
                            MotDePasse = hashedPassword,
                            Telephone = utilisateurInfo.Telephone.Trim(),
                            Roles = 0,
                            estPremiereConnection = 0,
                            AbonnementId = utilisateurInfo.Abonnement.AbonnementId,
                            AdresseId = utilisateurInfo.Adresse.AdresseId,
                            ImagePath = uniqueFileName,
                        };

                        db.Utilisateur.Add(NewUtilisateur);
                        db.SaveChanges();

                        if (SendMail(utilisateurInfo))
                        {
                            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
                            ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole");
                            return View(utilisateurInfo);
                        }
                        else
                        {
                            ViewBag.Message = "Erreur lors de l'envoi du message";
                            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
                            ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole");
                            return View(new Utilisateur());
                        }
                    }
                }
                else
                {
                    ViewBag.Message = "Erreur ! Le format de votre email n'est pas valide";
                    ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
                    ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole");
                    return View(new Utilisateur());
                }
            }

            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre", utilisateurInfo.AbonnementId);
            ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole", utilisateurInfo.Adresse.Province.ProvinceId);
            return View(utilisateurInfo);
        }


        private string GenererMotPasse(Utilisateur utilisateurInfo)
        {
            string nom = utilisateurInfo.Nom;
            string prenom = utilisateurInfo.Prenom;
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

        private bool ValiderMail(Utilisateur utilisateurInfo)
        {
            if (utilisateurInfo.Email.Contains("@") && utilisateurInfo.Email.Contains("."))
            {
                string[] parties = utilisateurInfo.Email.Split('@', '.');

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

        private bool SendMail(Utilisateur utilisateurInfo)
        {

            string suject = "Bienvenue sur GoFast";
            string Message = $"Bienvenue sur GoFast\n" +
                $"Voici les informations pour votre première connexion:\nEmail : {utilisateurInfo.Email}\nMot de passe : {GenererMotPasse(utilisateurInfo)}" 
               + $"\nUtiliser cette information pour vous conncetez " +
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
                MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, utilisateurInfo.Email, suject, Message);
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


        public ActionResult Connexion()
        {
            return View(new Utilisateur());
        }
        [HttpPost]
        public ActionResult Connexion(Utilisateur utilisateurInfo)
        {
            if (string.IsNullOrEmpty(utilisateurInfo.Email) || string.IsNullOrEmpty(utilisateurInfo.MotDePasse))
            {
                ViewBag.Message = "Erreur ! Veuillez remplir tous les champs.";
                return View();
            }
            string Email = utilisateurInfo.Email.Trim();
            Utilisateur utilisateur = db.Utilisateur.SingleOrDefault(m => m.Email == Email);
            if (utilisateur == null)
            {
                ViewBag.Message = "Erreur ! Informations invalides, veuillez réessayer.";
                return View();
            }
            else
            {
                string hashedPassword = utilisateur.MotDePasse;
                string passwordToCheck = utilisateurInfo.MotDePasse;

                // Vérifier le mot de passe
                bool passwordMatch = Crypto.VerifyHashedPassword(hashedPassword, passwordToCheck);

                if (passwordMatch)
                {
                    Utilisateur utilisateuraa = db.Utilisateur.SingleOrDefault(m => m.Email == utilisateurInfo.Email);
                    if (utilisateur.estPremiereConnection != 0)
                    {
                        Session["UtilisateurID"] = utilisateuraa.UtilisateurId;
                        return RedirectToAction("Acceuil");
                    }
                    else
                    {
                        Session["UtilisateurID"] = utilisateur.UtilisateurId;
                        return RedirectToAction("PremiereConnexion");
                    }
                }
                else if (utilisateurInfo.MotDePasse.Trim() == utilisateur.MotDePasse)
                {
                    Session["UtilisateurID"] = utilisateur.UtilisateurId;
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
            int id = Convert.ToInt32(Session["UtilisateurID"]);
            var optionsProfil = new List<SelectListItem>
            {
                new SelectListItem { Text = "PROFIL", Value = "", Selected = true },
                new SelectListItem { Text = "Voir Profil", Value = "Voir_Profil" },
                new SelectListItem { Text = "Supprimer Profil", Value = "Supprimer_Profil" },
            };

            ViewBag.Profil = optionsProfil;
            return View();
        }

        public ActionResult PremiereConnexion()
        {
            int id = Convert.ToInt32(Session["UtilisateurID"]);
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            if (utilisateur == null)
            {
                return HttpNotFound();
            }
            return View(new Utilisateur());
        }


        [HttpPost]
        public ActionResult PremiereConnexion(Utilisateur UtilisateurInfo)
        {
            int id = Convert.ToInt32(Session["UtilisateurID"]);
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            if (UtilisateurInfo.MotDePasse != null)
            {
                if (utilisateur == null)
                {
                    return HttpNotFound();
                }

                // Mettez à jour uniquement le mot de passe
                // To do : Crypter le mot de passe
                string hashedPassword = Crypto.HashPassword(UtilisateurInfo.MotDePasse);
                utilisateur.MotDePasse = hashedPassword;
                utilisateur.estPremiereConnection = 1;
                db.SaveChanges();
                return RedirectToAction("Connexion");
            }
            else
            {
                return RedirectToAction("Connexion");
            }
        }

        public ActionResult Deconnexion()
        {
            Session.Clear();
            return RedirectToAction("Connexion", "User");
        }


        public ActionResult Profil()
        {
            int id = Convert.ToInt32(Session["UtilisateurID"]);
            if (id == 0)
            {
                return RedirectToAction("Acceuil", "User");
            }
            Utilisateur utilisateur = db.Utilisateur.Find(id);
            if (utilisateur == null)
            {
                return RedirectToAction("Acceuil", "User");
            }
            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
            ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole");
            return View(utilisateur);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profil(Utilisateur utilisateur)
        {
            int id = Convert.ToInt32(Session["UtilisateurID"]);
            if (ModelState.IsValid)
            {
                db.Entry(utilisateur).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Acceuil", "Drivers");
            }
            ViewBag.AbonnementId = new SelectList(db.Abonnement, "AbonnementId", "Titre");
            ViewBag.ProvinceId = new SelectList(db.Province, "ProvinceId", "Symbole");
            return View(utilisateur);
        }


        public ActionResult MotPaaseOublie()
        {

            return View(new Utilisateur());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MotPaaseOublie(Utilisateur UtilisateurInfo)
        {
            if (!string.IsNullOrEmpty(UtilisateurInfo.Email))
            {
                Utilisateur Utili = db.Utilisateur.SingleOrDefault(x => x.Email == UtilisateurInfo.Email);

                if (Utili != null) //chauffeur existe deja 
                {
                    string motPasse = Utili.MotDePasse;
                    Session["motPasse"] = motPasse;

                    //To do : Envoyé email 
                    if (SendMailOublier(Utili) == true)
                    {
                        ViewBag.Message = "Verifier votre courriel, un mail a été envoyé !";
                        return View(Utili);
                    }
                    else
                    {
                        ViewBag.Message = "Erreur lors de l'envoie du message";
                        return View(Utili);
                    }
                }
                else
                {
                    ViewBag.Message = "Email introuvable, veuillez réessayer !";
                    return View();
                }
            }
            ViewBag.Message = "Veuillez remplir tous les champs requis.";
            return View(new Utilisateur());

        }

        private bool SendMailOublier(Utilisateur UtilisateurInfo)
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
                MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, UtilisateurInfo.Email, suject, Message);
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

        public ActionResult Voir_Profil()
        {
            int id = Convert.ToInt32(Session["UtilisateurID"]);
            var utilisateur = db.Utilisateur
                                .Include(u => u.Abonnement)
                                .Include(u => u.Adresse)
                                .Include(u => u.Adresse.Province)
                                .FirstOrDefault(m => m.UtilisateurId == id);


            return View(utilisateur);
        }


        public ActionResult DeleteProfil(int id)
        {
            Adresse Adresse = db.Adresse.Find(id);
            Utilisateur Utilisateur = db.Utilisateur.Find(id);
            Voiture Voiture = db.Voiture.Find(id);
            db.Utilisateur.Remove(Utilisateur);
            db.Voiture.Remove(Voiture);
            db.Adresse.Remove(Adresse);
            db.SaveChanges();
            return RedirectToAction("Connexion");
        }








    }








}