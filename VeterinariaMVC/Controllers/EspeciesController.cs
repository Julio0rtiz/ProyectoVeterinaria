using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador")]
public class EspeciesController : Controller
{
    private readonly AppDbContext _context;

    public EspeciesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _context.Especies
            .Include(e => e.Razas)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(e => e.Nombre.Contains(texto));
        }

        ViewBag.Q = q;
        return View(await query.OrderBy(e => e.Nombre).ToListAsync());
    }

    public IActionResult Crear()
    {
        return View(new Especie());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Especie especie)
    {
        especie.Nombre = (especie.Nombre ?? "").Trim();

        if (await _context.Especies.AnyAsync(e => e.Nombre == especie.Nombre))
        {
            ModelState.AddModelError(nameof(especie.Nombre), "Ya existe una especie con ese nombre.");
        }

        if (!ModelState.IsValid)
        {
            return View(especie);
        }

        _context.Especies.Add(especie);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Especie agregada.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var especie = await _context.Especies.FindAsync(id);
        if (especie is null)
        {
            return NotFound();
        }

        return View(especie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Especie especie)
    {
        if (id != especie.Id)
        {
            return BadRequest();
        }

        especie.Nombre = (especie.Nombre ?? "").Trim();

        if (await _context.Especies.AnyAsync(e => e.Nombre == especie.Nombre && e.Id != id))
        {
            ModelState.AddModelError(nameof(especie.Nombre), "Ya existe una especie con ese nombre.");
        }

        if (!ModelState.IsValid)
        {
            return View(especie);
        }

        var existente = await _context.Especies.FindAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        existente.Nombre = especie.Nombre;
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Especie actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var enUso = await _context.Razas.AnyAsync(r => r.EspecieId == id)
            || await _context.Mascotas.AnyAsync(m => m.EspecieId == id);

        if (enUso)
        {
            TempData["Error"] = "No se puede eliminar porque la especie tiene razas o mascotas asociadas.";
            return RedirectToAction(nameof(Index));
        }

        var especie = await _context.Especies.FindAsync(id);
        if (especie is null)
        {
            return NotFound();
        }

        try
        {
            _context.Especies.Remove(especie);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Especie eliminada.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar porque la especie tiene razas o mascotas asociadas.";
        }

        return RedirectToAction(nameof(Index));
    }
}