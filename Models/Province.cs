using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projet_GoFast.Models
{
    public class Province
    {
        [Key]
        public int ProvinceId { get; set; }
        public string Symbole { get; set; }
        public string Nom { get; set; }


        //Relation 1-n avec Chauffeur
        public virtual List<Adresse> Adresse { get; set; }
    }
}