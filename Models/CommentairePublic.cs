using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace Projet_GoFast.Models
{
    public class CommentairePublic
    {
        [Key]
        public int CommentairePublicID  { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string Nom { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        [Required(ErrorMessage = "⚠️")]
        public string Commentaire { get; set; }
    }
}