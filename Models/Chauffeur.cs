using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Chauffeur
    {
        [Key]
        public int ChauffeurId { get; set; }
        [Required(ErrorMessage = "⚠️")]
     

        //Information personnelle
        public string Nom { get; set; }

        [Required(ErrorMessage = "⚠️")]

        public string Prenom { get; set; }

        [Required(ErrorMessage = "⚠️")]
        public string Email { get; set; }
        [Required(ErrorMessage = "⚠️")]
  

        public string Telephone { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string MotDePasse { get; set; }

        public int? Roles { get; set; }

        //connexion
        public int estPremiereConnection { get; set; }

        //Relation avec voiture 
        public int VoitureId { get; set; }
        public virtual Voiture Voiture { get; set; }


        //Relation avec Adresse 
        public int AdresseId { get; set; }
        public virtual Adresse Adresse { get; set; }

        public string ImagePath { get; set; } 
        [NotMapped]
        public HttpPostedFileBase ImageFile { get; set; }

    }
}