using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Abonnement
    {
        [Key]
        public int AbonnementId { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public decimal Prix { get; set; }

        // Relation 1-n Utilisateur
        public virtual List<Utilisateur> Utilisateur { get; set; }


    }
}