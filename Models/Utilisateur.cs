using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Utilisateur
    {
        [Key]
        public int UtilisateurId { get; set; }
        [Required(ErrorMessage = "⚠️")]

        //Information personnelle------------------
        public string Nom { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string Prenom { get; set; }
        [Required(ErrorMessage = "⚠️")]
        [EmailAddress(ErrorMessage = "L'email n'est pas au bon format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string MotDePasse { get; set; }

        public int? Roles { get; set; }


        //Relation 1-n avec Adresse
        public int AdresseId { get; set; }
        public virtual Adresse Adresse { get; set; }


        // Relation 1-n Abonnement
        public int AbonnementId { get; set; }
        public virtual Abonnement Abonnement { get; set; }

        //connexion
        public int estPremiereConnection { get; set; }


        public string ImagePath { get; set; } // Chemin de l'image sauvegardée
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}