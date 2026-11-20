using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Services;

namespace VeterinariaMVC.Controllers;

[ApiController]
[Route("api/examenes")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Dueno")]
public class ExamenesApiController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IArchivoService _archivos;

    public ExamenesApiController(AppDbContext context, IArchivoService archivos)
    {
        _context = context;
        _archivos = archivos;
    }

    [HttpGet("{id:int}/archivo")]
    public async Task<IActionResult> Archivo(int id)
    {
        var duenoId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var examen = await _context.Examenes
            .Include(e => e.Mascota)
            .FirstOrDefaultAsync(e => e.Id == id && e.Mascota!.DuenoId == duenoId);

        if (examen is null)
        {
            return NotFound(new { mensaje = "Examen no encontrado." });
        }

        var (ruta, tipo) = _archivos.ObtenerPrivado(examen.ArchivoRuta);
        return PhysicalFile(ruta, tipo, examen.NombreOriginal);
    }
}