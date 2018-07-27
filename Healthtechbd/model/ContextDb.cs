namespace Healthtechbd.model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ContextDb : DbContext
    {
        public ContextDb()
            : base("name=ContextDb")
        {
        }

        public virtual DbSet<test> tests { get; set; }
        public virtual DbSet<medicine> medicines { get; set; }
        public virtual DbSet<diagnosis> diagnosis { get; set; }
        public virtual DbSet<diagnosis_templates> diagnosis_templates { get; set; }
        public virtual DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<test>()
                .Property(e => e.name)
                .IsUnicode(false);
        }
    }
}
