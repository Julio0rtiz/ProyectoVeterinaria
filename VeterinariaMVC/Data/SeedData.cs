using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Data;

public static class SeedData
{
    public static void Inicializar(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();

        db.Database.Migrate();

        if (db.Usuarios.Any())
        {
            return;
        }

        var veterinario = new Veterinario
        {
            NombreCompleto = "Dra. Ana Rojas",
            NumeroColegiado = "MV-1001"
        };
        db.Veterinarios.Add(veterinario);

        db.Usuarios.AddRange(
            CrearUsuario(hasher, "100000001", "Administrador", "admin@veterinaria.com", "Admin123!", Perfil.Administrador),
            CrearUsuario(hasher, "100000002", "Ana Rojas", "vet@veterinaria.com", "Vet123!", Perfil.Veterinario, veterinario),
            CrearUsuario(hasher, "100000003", "Carlos Mora", "dueno@veterinaria.com", "Dueno123!", Perfil.Dueno));

        db.SaveChanges();
    }

    private static Usuario CrearUsuario(IPasswordHasher<Usuario> hasher, string cedula, string nombre,
        string correo, string contrasena, Perfil perfil, Veterinario? veterinario = null)
    {
        var usuario = new Usuario
        {
            Cedula = cedula,
            Nombre = nombre,
            Correo = correo,
            Perfil = perfil,
            Veterinario = veterinario
        };
        usuario.ContrasenaHash = hasher.HashPassword(usuario, contrasena);
        return usuario;
    }
}
