using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador,Veterinario")]
public class VeterinariaController : ExpedienteBaseController
{
    public VeterinariaController(AppDbContext context) : base(context)
    {
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _context.Mascotas
            .Include(m => m.Especie)
            .Include(m => m.Raza)
            .Include(m => m.Dueno)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(m => m.Nombre.Contains(texto) || m.Dueno!.Cedula.Contains(texto));
        }

        var lista = await query
            .OrderBy(m => m.UltimaAtencion == null)
            .ThenByDescending(m => m.UltimaAtencion)
            .ThenBy(m => m.Nombre)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(q) && lista.Count == 1)
        {
            return RedirectToAction("Ver", "Expediente", new { id = lista[0].Id });
        }

        ViewBag.Q = q;
        return View(lista);
    }
}
