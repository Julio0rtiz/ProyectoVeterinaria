using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VeterinariaMVC.Data;

namespace VeterinariaMVC.Controllers;

public abstract class ExpedienteBaseController : Controller
{
    protected readonly AppDbContext _context;

    protected ExpedienteBaseController(AppDbContext context)
    {
        _context = context;
    }

    protected int UsuarioActualId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    protected async Task RegistrarAtencionAsync(int mascotaId)
    {
        var mascota = await _context.Mascotas.FindAsync(mascotaId);
        if (mascota is not null)
        {
            mascota.UltimaAtencion = DateTime.Now;
        }
    }

    protected string PrimerError()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "Verifique los datos ingresados.";
    }
}
