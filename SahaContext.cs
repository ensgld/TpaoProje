using Microsoft.EntityFrameworkCore;

public class SahaContext : DbContext
{
    public DbSet<Saha> Sahalar { get; set; }
    public DbSet<Kuyu> Kuyular { get; set; }
    public DbSet<Wellbore> Wellbores { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Saha>()
            .HasMany(s => s.kuyuList)
            .WithOne(k => k.Saha)
            .HasForeignKey(k => k.SahaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Kuyu>()
            .HasMany(k => k.wellbores)
            .WithOne(w => w.Kuyu)
            .HasForeignKey(w => w.KuyuId)
            .OnDelete(DeleteBehavior.Cascade);
    }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS;Initial Catalog=TPAOProjeDb;Integrated Security=True;TrustServerCertificate=True;");
    }

}