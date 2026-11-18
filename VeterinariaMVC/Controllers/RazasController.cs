using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador")]
public class RazasController : Controller
{
    private readonly AppDbContext _context;

    public RazasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? especieId, string? q)
    {
        var query = _context.Razas
            .Include(r => r.Especie)
            .Include(r => r.Mascotas)
            .AsQueryable();

        if (especieId.HasValue && especieId.Value > 0)
        {
            query = query.Where(r => r.EspecieId == especieId.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(r => r.Nombre.Contains(texto));
        }

        await CargarEspeciesAsync(especieId);
        ViewBag.EspecieId = especieId;
        ViewBag.Q = q;

        var lista = await query
            .OrderBy(r => r.Especie!.Nombre)
            .ThenBy(r => r.Nombre)
            .ToListAsync();

        return View(lista);
    }

    public async Task<IActionResult> Crear(int? especieId)
    {
        await CargarEspeciesAsync(especieId);
        return View(new Raza { EspecieId = especieId ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Raza raza)
    {
        raza.Nombre = (raza.Nombre ?? "").Trim();

        if (raza.EspecieId > 0 && !await _context.Especies.AnyAsync(e => e.Id == raza.EspecieId))
        {
            ModelState.AddModelError(nameof(raza.EspecieId), "Seleccione una especie válida.");
        }

        if (raza.EspecieId > 0 && await _context.Razas.AnyAsync(r => r.EspecieId == raza.EspecieId && r.Nombre == raza.Nombre))
        {
            ModelState.AddModelError(nameof(raza.Nombre), "Ya existe una raza con ese nombre en la especie seleccionada.");
        }

        if (!ModelState.IsValid)
        {
            await CargarEspeciesAsync(raza.EspecieId);
            return View(raza);
        }

        _context.Razas.Add(raza);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Raza agregada.";
        return RedirectToAction(nameof(Index), new { especieId = raza.EspecieId });
    }

    public async Task<IActionResult> Editar(int id)
    {
        var raza = await _context.Razas.FindAsync(id);
        if (raza is null)
        {
            return NotFound();
        }

        await CargarEspeciesAsync(raza.EspecieId);
        return View(raza);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Raza raza)
    {
        if (id != raza.Id)
        {
            return BadRequest();
        }

        raza.Nombre = (raza.Nombre ?? "").Trim();

        if (raza.EspecieId > 0 && !await _context.Especies.AnyAsync(e => e.Id == raza.EspecieId))
        {
            ModelState.AddModelError(nameof(raza.EspecieId), "Seleccione una especie válida.");
        }

        if (raza.EspecieId > 0 && await _context.Razas.AnyAsync(r => r.EspecieId == raza.EspecieId && r.Nombre == raza.Nombre && r.Id != id))
        {
            ModelState.AddModelError(nameof(raza.Nombre), "Ya existe una raza con ese nombre en la especie seleccionada.");
        }

        if (!ModelState.IsValid)
        {
            await CargarEspeciesAsync(raza.EspecieId);
            return View(raza);
        }

        var existente = await _context.Razas.FindAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        existente.Nombre = raza.Nombre;
        existente.EspecieId = raza.EspecieId;
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Raza actualizada.";
        return RedirectToAction(nameof(Index), new { especieId = existente.EspecieId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var raza = await _context.Razas.FindAsync(id);
        if (raza is null)
        {
            return NotFound();
        }

        var enUso = await _context.Mascotas.AnyAsync(m => m.RazaId == id);
        if (enUso)
        {
            TempData["Error"] = "No se puede eliminar: la raza tiene mascotas asociadas.";
            return RedirectToAction(nameof(Index), new { especieId = raza.EspecieId });
        }

        var especieId = raza.EspecieId;

        try
        {
            _context.Razas.Remove(raza);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Raza eliminada.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar: la raza tiene mascotas asociadas.";
        }

        return RedirectToAction(nameof(Index), new { especieId });
    }

    private async Task CargarEspeciesAsync(int? seleccionada = null)
    {
        var especies = await _context.Especies
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        ViewBag.Especies = new SelectList(especies, nameof(Especie.Id), nameof(Especie.Nombre), seleccionada);
    }
}