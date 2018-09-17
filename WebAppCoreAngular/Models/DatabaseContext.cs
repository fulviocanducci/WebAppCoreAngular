using Microsoft.EntityFrameworkCore;
namespace WebAppCoreAngular.Models
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=db.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<People>(x =>
            {
                x.HasKey(p => p.Id);
                x.Property(p => p.Id)
                    .IsRequired();
                x.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                x.HasMany(p => p.Phones)
                    .WithOne(p => p.People)
                    .HasForeignKey(k => k.PeopleId)
                    .HasPrincipalKey(k => k.Id);                    
            });

            modelBuilder.Entity<Phone>(x =>
            {
                x.HasKey(p => p.Id);
                x.Property(p => p.Id)
                    .IsRequired();
                x.Property(p => p.PeopleId)
                    .IsRequired();
                x.Property(p => p.Ddd)
                    .IsRequired()
                    .HasMaxLength(3);
                x.Property(p => p.Number)
                    .IsRequired()
                    .HasMaxLength(10);
                x.HasOne(p => p.People)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(k => k.PeopleId)
                    .HasPrincipalKey(k => k.Id);
            });
        }

        public DbSet<People> People { get; set; }
        public DbSet<Phone> Phone { get; set; }
    }
}
