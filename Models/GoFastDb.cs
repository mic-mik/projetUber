using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Projet_GoFast.Models
{
    public class GoFastDb :DbContext
    {
        public DbSet<Utilisateur> Utilisateur { get; set; }
        public DbSet<Abonnement> Abonnement { get; set; }

        public DbSet<Province> Province { get; set; }
        public DbSet<Chauffeur> Chauffeur { get; set; }
        public DbSet<Marque> Marque { get; set; }
        public DbSet<Voiture> Voiture { get; set; }

        public DbSet<Adresse> Adresse { get; set; }
        public DbSet<CommentairePublic> CommentairePublic { get; set; }
        

        public GoFastDb()
        {
           Database.SetInitializer(new MigrateDatabaseToLatestVersion<Projet_GoFast.Models.GoFastDb, Projet_GoFast.Migrations.Configuration>());
        }

    }
}