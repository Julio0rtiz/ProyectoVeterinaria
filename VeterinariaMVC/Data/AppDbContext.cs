using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Veterinario> Veterinarios { get; set; }
    public DbSet<Especie> Especies { get; set; }
    public DbSet<Raza> Razas { get; set; }
    public DbSet<ItemCatalogo> Catalogo { get; set; }
    public DbSet<Mascota> Mascotas { get; set; }
    public DbSet<Vacuna> Vacunas { get; set; }
    public DbSet<TratamientoAsignado> Tratamientos { get; set; }
    public DbSet<Examen> Examenes { get; set; }
    public DbSet<NotaHistorial> Notas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().HasIndex(u => u.Cedula).IsUnique();
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Correo).IsUnique();
        modelBuilder.Entity<Veterinario>().HasIndex(v => v.NumeroColegiado).IsUnique();
        modelBuilder.Entity<Especie>().HasIndex(e => e.Nombre).IsUnique();
        modelBuilder.Entity<Raza>().HasIndex(r => new { r.EspecieId, r.Nombre }).IsUnique();

        modelBuilder.Entity<Mascota>()
            .HasOne(m => m.Dueno)
            .WithMany(u => u.Mascotas)
            .HasForeignKey(m => m.DuenoId);

        modelBuilder.Entity<TratamientoAsignado>()
            .HasOne(t => t.Autor)
            .WithMany()
            .HasForeignKey(t => t.AutorId);

        modelBuilder.Entity<TratamientoAsignado>()
            .HasOne(t => t.AutorSuspension)
            .WithMany()
            .HasForeignKey(t => t.AutorSuspensionId);

        modelBuilder.Entity<TratamientoAsignado>()
            .HasOne(t => t.ItemCatalogo)
            .WithMany(i => i.Asignaciones)
            .HasForeignKey(t => t.ItemCatalogoId);

        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
