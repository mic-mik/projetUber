using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Voiture
    {
        [Key]
        public int VoitureId { get; set; }

        //Relation 1--n avec Marque

        public int MarqueId { get; set; }
        public virtual Marque Marque { get; set; }

        [Required(ErrorMessage = "⚠️")]
        public string Modele { get; set; }

        [Required(ErrorMessage = "⚠️")]
        [Range(2000, 2024, ErrorMessage = "Veuillez entrer une année valide.")]
        public int Annee { get; set; }

        [Required(ErrorMessage = "⚠️")]
        public String Immatriculation { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public String PermisConduite { get; set; }

        //Relation avec chaffeur 
        public virtual List<Chauffeur> Chauffeur  { get; set; }

    }
}