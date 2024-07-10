using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Adresse
    {
        [Key]
        public int AdresseId { get; set; }

        [Required(ErrorMessage = "⚠️")]
        public string Rue { get; set; }

        [Required(ErrorMessage = "⚠️")]
        public string Ville { get; set; }

        [Required(ErrorMessage = "⚠️")]
        public string CodePostal { get; set; }

        //Relation 1-n avec Province

        public int ProvinceId { get; set; }
        public virtual Province Province { get; set; }

        //Relation avec utilisateur 
        public virtual List<Utilisateur> Utilisateur { get; set; }

        //Relation avec chaffeur 
        public virtual List<Chauffeur> Chauffeur { get; set; }
    }
}