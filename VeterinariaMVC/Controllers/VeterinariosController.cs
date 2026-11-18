using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.Models.ViewModels;
using VeterinariaMVC.Services;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador")]
public class VeterinariosController : Controller
{
    private readonly AppDbContext _context;
    private readonly IArchivoService _archivos;

    public VeterinariosController(AppDbContext context, IArchivoService archivos)
    {
        _context = context;
        _archivos = archivos;
    }

    public async Task<IActionResult> Index(string? q)
    {
        var query = _context.Veterinarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(v => v.NombreCompleto.Contains(texto) || v.NumeroColegiado.Contains(texto));
        }

        var vinculados = await _context.Usuarios
            .Where(u => u.VeterinarioId != null)
            .Select(u => u.VeterinarioId!.Value)
            .ToListAsync();

        ViewBag.Q = q;
        ViewBag.Vinculados = vinculados.ToHashSet();

        return View(await query.OrderBy(v => v.NombreCompleto).ToListAsync());
    }

    public IActionResult Crear()
    {
        return View(new VeterinarioFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(VeterinarioFormViewModel vm)
    {
        vm.NombreCompleto = (vm.NombreCompleto ?? "").Trim();
        vm.NumeroColegiado = (vm.NumeroColegiado ?? "").Trim();

        if (await _context.Veterinarios.AnyAsync(v => v.NumeroColegiado == vm.NumeroColegiado))
        {
            ModelState.AddModelError(nameof(vm.NumeroColegiado), "Ya existe un veterinario con ese número de colegiado.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var veterinario = new Veterinario
        {
            NombreCompleto = vm.NombreCompleto,
            NumeroColegiado = vm.NumeroColegiado
        };

        try
        {
            if (vm.Foto is not null)
            {
                veterinario.FotoRuta = await _archivos.GuardarFotoAsync(vm.Foto, "veterinarios");
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(vm.Foto), ex.Message);
            return View(vm);
        }

        _context.Veterinarios.Add(veterinario);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Veterinario agregado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var veterinario = await _context.Veterinarios.FindAsync(id);
        if (veterinario is null)
        {
            return NotFound();
        }

        var vm = new VeterinarioFormViewModel
        {
            Id = veterinario.Id,
            NombreCompleto = veterinario.NombreCompleto,
            NumeroColegiado = veterinario.NumeroColegiado,
            FotoActual = veterinario.FotoRuta
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, VeterinarioFormViewModel vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        var existente = await _context.Veterinarios.FindAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        vm.NombreCompleto = (vm.NombreCompleto ?? "").Trim();
        vm.NumeroColegiado = (vm.NumeroColegiado ?? "").Trim();
        vm.FotoActual = existente.FotoRuta;

        if (await _context.Veterinarios.AnyAsync(v => v.NumeroColegiado == vm.NumeroColegiado && v.Id != id))
        {
            ModelState.AddModelError(nameof(vm.NumeroColegiado), "Ya existe un veterinario con ese número de colegiado.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var rutaAnterior = existente.FotoRuta;

        try
        {
            if (vm.Foto is not null)
            {
                existente.FotoRuta = await _archivos.GuardarFotoAsync(vm.Foto, "veterinarios");
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(vm.Foto), ex.Message);
            return View(vm);
        }

        existente.NombreCompleto = vm.NombreCompleto;
        existente.NumeroColegiado = vm.NumeroColegiado;
        await _context.SaveChangesAsync();

        if (vm.Foto is not null && !string.IsNullOrWhiteSpace(rutaAnterior))
        {
            _archivos.Eliminar(rutaAnterior);
        }

        TempData["Exito"] = "Veterinario actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var enUso = await _context.Usuarios.AnyAsync(u => u.VeterinarioId == id);
        if (enUso)
        {
            TempData["Error"] = "No se puede eliminar el veterinario porque tiene un usuario vinculado.";
            return RedirectToAction(nameof(Index));
        }

        var veterinario = await _context.Veterinarios.FindAsync(id);
        if (veterinario is null)
        {
            return NotFound();
        }

        var fotoRuta = veterinario.FotoRuta;

        try
        {
            _context.Veterinarios.Remove(veterinario);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(fotoRuta))
            {
                _archivos.Eliminar(fotoRuta);
            }

            TempData["Exito"] = "Veterinario eliminado.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar el veterinario porque tiene un usuario vinculado.";
        }

        return RedirectToAction(nameof(Index));
    }
}