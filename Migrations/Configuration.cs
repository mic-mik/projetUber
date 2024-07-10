namespace Projet_GoFast.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Projet_GoFast.Models.GoFastDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Projet_GoFast.Models.GoFastDb";
            AutomaticMigrationDataLossAllowed = true;

        }

        protected override void Seed(Projet_GoFast.Models.GoFastDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
