using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.Models.ViewModels;
using VeterinariaMVC.Services;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador,Veterinario")]
public class ExpedienteController : ExpedienteBaseController
{
    private static readonly string[] Pestanas = { "vacunas", "tratamientos", "examenes", "historial" };

    private readonly IArchivoService _archivos;

    public ExpedienteController(AppDbContext context, IArchivoService archivos) : base(context)
    {
        _archivos = archivos;
    }

    public async Task<IActionResult> Ver(int id, string tab = "vacunas")
    {
        var mascota = await _context.Mascotas
            .Include(x => x.Especie)
            .Include(x => x.Raza)
            .Include(x => x.Dueno)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (mascota is null)
        {
            return NotFound();
        }

        var vm = new ExpedienteViewModel
        {
            Mascota = mascota,
            Tab = Pestanas.Contains(tab) ? tab : "vacunas",
            Vacunas = await _context.Vacunas
                .Where(v => v.MascotaId == id)
                .Include(v => v.Autor).ThenInclude(a => a!.Veterinario)
                .OrderByDescending(v => v.FechaAplicacion)
                .ThenByDescending(v => v.FechaRegistro)
                .ToListAsync(),
            Tratamientos = await _context.Tratamientos
                .Where(t => t.MascotaId == id)
                .Include(t => t.ItemCatalogo)
                .Include(t => t.Autor).ThenInclude(a => a!.Veterinario)
                .Include(t => t.AutorSuspension).ThenInclude(a => a!.Veterinario)
                .OrderByDescending(t => t.FechaInicio)
                .ThenByDescending(t => t.Id)
                .ToListAsync(),
            Examenes = await _context.Examenes
                .Where(e => e.MascotaId == id)
                .Include(e => e.Autor).ThenInclude(a => a!.Veterinario)
                .OrderByDescending(e => e.FechaRegistro)
                .ToListAsync(),
            Notas = await _context.Notas
                .Where(n => n.MascotaId == id)
                .Include(n => n.Autor).ThenInclude(a => a!.Veterinario)
                .OrderByDescending(n => n.FechaHora)
                .ToListAsync(),
            Catalogo = await _context.Catalogo
                .OrderBy(c => c.Tipo)
                .ThenBy(c => c.Nombre)
                .ToListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarVacuna(int mascotaId, VacunaFormViewModel vm)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == mascotaId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = PrimerError();
        }
        else if (vm.FechaAplicacion!.Value.Date > DateTime.Today)
        {
            TempData["Error"] = "La fecha de aplicación no puede ser futura.";
        }
        else if (vm.FechaProximaDosis!.Value.Date <= vm.FechaAplicacion.Value.Date)
        {
            TempData["Error"] = "La próxima dosis debe ser posterior a la aplicación.";
        }
        else
        {
            _context.Vacunas.Add(new Vacuna
            {
                MascotaId = mascotaId,
                Nombre = vm.Nombre.Trim(),
                FechaAplicacion = vm.FechaAplicacion.Value.Date,
                FechaProximaDosis = vm.FechaProximaDosis.Value.Date,
                AutorId = UsuarioActualId,
                FechaRegistro = DateTime.Now
            });
            await RegistrarAtencionAsync(mascotaId);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Vacuna registrada.";
        }

        return RedirectToAction(nameof(Ver), new { id = mascotaId, tab = "vacunas" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarTratamiento(int mascotaId, TratamientoFormViewModel vm)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == mascotaId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = PrimerError();
        }
        else if (!await _context.Catalogo.AnyAsync(c => c.Id == vm.ItemCatalogoId))
        {
            TempData["Error"] = "Seleccione un medicamento o tratamiento válido del catálogo.";
        }
        else if (await _context.Tratamientos.AnyAsync(t =>
                     t.MascotaId == mascotaId && t.ItemCatalogoId == vm.ItemCatalogoId && !t.Suspendido))
        {
            TempData["Error"] = "Ese medicamento o tratamiento ya está vigente para esta mascota. Suspéndalo antes de asignarlo de nuevo.";
        }
        else
        {
            _context.Tratamientos.Add(new TratamientoAsignado
            {
                MascotaId = mascotaId,
                ItemCatalogoId = vm.ItemCatalogoId,
                Indicaciones = vm.Indicaciones.Trim(),
                FechaInicio = vm.FechaInicio!.Value.Date,
                AutorId = UsuarioActualId,
                Suspendido = false
            });
            await RegistrarAtencionAsync(mascotaId);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Tratamiento asignado.";
        }

        return RedirectToAction(nameof(Ver), new { id = mascotaId, tab = "tratamientos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuspenderTratamiento(int id)
    {
        var tratamiento = await _context.Tratamientos.FindAsync(id);
        if (tratamiento is null)
        {
            return NotFound();
        }

        if (tratamiento.Suspendido)
        {
            TempData["Error"] = "El tratamiento ya estaba suspendido.";
        }
        else
        {
            tratamiento.Suspendido = true;
            tratamiento.FechaSuspension = DateTime.Now;
            tratamiento.AutorSuspensionId = UsuarioActualId;
            await RegistrarAtencionAsync(tratamiento.MascotaId);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Tratamiento suspendido.";
        }

        return RedirectToAction(nameof(Ver), new { id = tratamiento.MascotaId, tab = "tratamientos" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(60 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 60 * 1024 * 1024)]
    public async Task<IActionResult> AgregarExamen(int mascotaId, ExamenFormViewModel vm)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == mascotaId))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = PrimerError();
        }
        else
        {
            try
            {
                var ruta = await _archivos.GuardarPrivadoAsync(vm.Archivo!, "examenes");

                _context.Examenes.Add(new Examen
                {
                    MascotaId = mascotaId,
                    Descripcion = vm.Descripcion.Trim(),
                    ArchivoRuta = ruta,
                    NombreOriginal = Path.GetFileName(vm.Archivo!.FileName),
                    TipoContenido = _archivos.ObtenerPrivado(ruta).tipoContenido,
                    FechaRegistro = DateTime.Now,
                    AutorId = UsuarioActualId
                });
                await RegistrarAtencionAsync(mascotaId);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Examen registrado.";
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        return RedirectToAction(nameof(Ver), new { id = mascotaId, tab = "examenes" });
    }

    public async Task<IActionResult> Archivo(int id)
    {
        var examen = await _context.Examenes.FindAsync(id);
        if (examen is null)
        {
            return NotFound();
        }

        try
        {
            var (ruta, tipo) = _archivos.ObtenerPrivado(examen.ArchivoRuta);
            return PhysicalFile(ruta, tipo);
        }
        catch (Exception ex) when (ex is FileNotFoundException or ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarNota(int mascotaId, string? contenidoHtml)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == mascotaId))
        {
            return NotFound();
        }

        var limpio = new HtmlSanitizer().Sanitize(contenidoHtml ?? "");
        var texto = Regex.Replace(limpio, "<.*?>", "").Replace("&nbsp;", " ").Trim();

        if (texto.Length == 0)
        {
            TempData["Error"] = "La nota no puede estar vacía.";
        }
        else
        {
            _context.Notas.Add(new NotaHistorial
            {
                MascotaId = mascotaId,
                ContenidoHtml = limpio,
                FechaHora = DateTime.Now,
                AutorId = UsuarioActualId
            });
            await RegistrarAtencionAsync(mascotaId);
            await _context.SaveChangesAsync();
            TempData["Exito"] = "Nota agregada al historial.";
        }

        return RedirectToAction(nameof(Ver), new { id = mascotaId, tab = "historial" });
    }
}
