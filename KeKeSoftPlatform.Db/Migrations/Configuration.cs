namespace KeKeSoftPlatform.Db.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using KeKeSoftPlatform.Common;

    internal sealed class Configuration : DbMigrationsConfiguration<KeKeSoftPlatform.Db.KeKeSoftPlatformDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "KeKeSoftPlatform.Db.KeKeSoftPlatformDbContext";

            this.CommandTimeout = 1800;
        }

        protected override void Seed(KeKeSoftPlatform.Db.KeKeSoftPlatformDbContext context)
        {
        }
    }
}
