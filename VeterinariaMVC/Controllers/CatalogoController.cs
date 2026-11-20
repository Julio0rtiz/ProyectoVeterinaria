using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador")]
public class CatalogoController : Controller
{
    private readonly AppDbContext _context;

    public CatalogoController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(TipoItemCatalogo? tipo, string? q)
    {
        var query = _context.Catalogo
            .Include(c => c.Asignaciones)
            .AsQueryable();

        if (tipo.HasValue && Enum.IsDefined(typeof(TipoItemCatalogo), tipo.Value))
        {
            query = query.Where(c => c.Tipo == tipo.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(c => c.Nombre.Contains(texto) || c.Descripcion.Contains(texto));
        }

        ViewBag.Tipo = tipo;
        ViewBag.Q = q;

        var lista = await query
            .OrderBy(c => c.Tipo)
            .ThenBy(c => c.Nombre)
            .ToListAsync();

        return View(lista);
    }

    public IActionResult Crear()
    {
        return View(new ItemCatalogo());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ItemCatalogo item)
    {
        item.Nombre = (item.Nombre ?? "").Trim();
        item.Descripcion = (item.Descripcion ?? "").Trim();

        if (!Enum.IsDefined(typeof(TipoItemCatalogo), item.Tipo))
        {
            ModelState.AddModelError(nameof(item.Tipo), "Seleccione un tipo válido.");
        }

        if (await _context.Catalogo.AnyAsync(c => c.Nombre == item.Nombre && c.Tipo == item.Tipo))
        {
            ModelState.AddModelError(nameof(item.Nombre), "Ya existe un registro con ese nombre para el tipo seleccionado.");
        }

        if (!ModelState.IsValid)
        {
            return View(item);
        }

        _context.Catalogo.Add(item);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Se agrego correctamente al catálogo clínico.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var item = await _context.Catalogo.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, ItemCatalogo item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }

        item.Nombre = (item.Nombre ?? "").Trim();
        item.Descripcion = (item.Descripcion ?? "").Trim();

        if (!Enum.IsDefined(typeof(TipoItemCatalogo), item.Tipo))
        {
            ModelState.AddModelError(nameof(item.Tipo), "Seleccione un tipo válido.");
        }

        if (await _context.Catalogo.AnyAsync(c => c.Nombre == item.Nombre && c.Tipo == item.Tipo && c.Id != id))
        {
            ModelState.AddModelError(nameof(item.Nombre), "Ya existe un registro con ese nombre para el tipo seleccionado.");
        }

        if (!ModelState.IsValid)
        {
            return View(item);
        }

        var existente = await _context.Catalogo.FindAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        existente.Nombre = item.Nombre;
        existente.Tipo = item.Tipo;
        existente.Descripcion = item.Descripcion;
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Se guardaron correctamente los cambios en el catálogo.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var enUso = await _context.Tratamientos.AnyAsync(t => t.ItemCatalogoId == id);
        if (enUso)
        {
            TempData["Error"] = "No se puede eliminar el medicamento o tratamiento ya fue asignado a una mascota.";
            return RedirectToAction(nameof(Index));
        }

        var item = await _context.Catalogo.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        try
        {
            _context.Catalogo.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Se ha eliminado correctamente del catálogo clínico.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar el medicamento o tratamiento ya fue asignado a una mascota.";
        }

        return RedirectToAction(nameof(Index));
    }
}