using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Marque
    {
        [Key]
        public int MarqueId { get; set; }
        public string Titre { get; set; }
        // Relation 1-n a avec voiture
        public virtual List<Voiture> Voiture { get; set; }
    }
}