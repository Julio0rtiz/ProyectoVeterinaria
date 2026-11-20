using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models.ViewModels;

namespace VeterinariaMVC.Controllers;

[ApiController]
[Route("api/mascotas")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Dueno")]
public class MascotasApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public MascotasApiController(AppDbContext context)
    {
        _context = context;
    }

    private int DuenoId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string Base => $"{Request.Scheme}://{Request.Host}/";

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> MisMascotas()
    {
        return Ok(await _context.Mascotas
            .Where(m => m.DuenoId == DuenoId)
            .OrderBy(m => m.Nombre)
            .Select(m => new MascotaDto(
                m.Id,
                m.Nombre,
                m.Especie!.Nombre,
                m.Raza!.Nombre,
                m.FechaNacimiento,
                m.Sexo.ToString(),
                m.FotoRuta == null ? null : Base + m.FotoRuta))
            .ToListAsync());
    }

    [HttpGet("{id:int}/vacunas")]
    public async Task<IActionResult> Vacunas(int id)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == id && m.DuenoId == DuenoId))
        {
            return NotFound(new { mensaje = "Mascota no encontrada." });
        }

        var hoy = DateTime.Today;
        return Ok(await _context.Vacunas
            .Where(v => v.MascotaId == id)
            .OrderByDescending(v => v.FechaAplicacion)
            .Select(v => new VacunaDto(
                v.Id,
                v.Nombre,
                v.FechaAplicacion,
                v.FechaProximaDosis,
                v.FechaProximaDosis < hoy,
                v.Autor!.Veterinario != null ? v.Autor.Veterinario.NombreCompleto : v.Autor.Nombre))
            .ToListAsync());
    }

    [HttpGet("{id:int}/examenes")]
    public async Task<IActionResult> Examenes(int id)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == id && m.DuenoId == DuenoId))
        {
            return NotFound(new { mensaje = "Mascota no encontrada." });
        }

        return Ok(await _context.Examenes
            .Where(e => e.MascotaId == id)
            .OrderByDescending(e => e.FechaRegistro)
            .Select(e => new ExamenDto(
                e.Id,
                e.Descripcion,
                e.FechaRegistro,
                e.NombreOriginal,
                e.TipoContenido,
                e.Autor!.Veterinario != null ? e.Autor.Veterinario.NombreCompleto : e.Autor.Nombre))
            .ToListAsync());
    }

    [HttpGet("{id:int}/tratamientos")]
    public async Task<IActionResult> Tratamientos(int id)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == id && m.DuenoId == DuenoId))
        {
            return NotFound(new { mensaje = "Mascota no encontrada." });
        }

        var lista = await _context.Tratamientos
            .Where(t => t.MascotaId == id)
            .OrderByDescending(t => t.FechaInicio)
            .Select(t => new TratamientoDto(
                t.Id,
                t.ItemCatalogo!.Nombre,
                t.ItemCatalogo.Tipo.ToString(),
                t.Indicaciones,
                t.FechaInicio,
                t.FechaSuspension,
                t.Suspendido,
                t.Autor!.Veterinario != null ? t.Autor.Veterinario.NombreCompleto : t.Autor.Nombre))
            .ToListAsync();

        var vigentes = lista.Where(t => !t.Suspendido).ToList();
        var suspendidos = lista.Where(t => t.Suspendido).ToList();

        return Ok(new { vigentes, suspendidos });
    }

    [HttpGet("{id:int}/historial")]
    public async Task<IActionResult> Historial(int id)
    {
        if (!await _context.Mascotas.AnyAsync(m => m.Id == id && m.DuenoId == DuenoId))
        {
            return NotFound(new { mensaje = "Mascota no encontrada." });
        }

        return Ok(await _context.Notas
            .Where(n => n.MascotaId == id)
            .OrderByDescending(n => n.FechaHora)
            .Select(n => new HistorialDto(
                n.FechaHora,
                n.ContenidoHtml,
                n.Autor!.Veterinario != null ? n.Autor.Veterinario.NombreCompleto : n.Autor.Nombre))
            .ToListAsync());
    }
}

