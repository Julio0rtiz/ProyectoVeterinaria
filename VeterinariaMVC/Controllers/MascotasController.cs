using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.Models.ViewModels;
using VeterinariaMVC.Services;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador,Veterinario")]
public class MascotasController : Controller
{
    private readonly AppDbContext _context;
    private readonly IArchivoService _archivos;
    private readonly IPasswordHasher<Usuario> _hasher;

    public MascotasController(
        AppDbContext context,
        IArchivoService archivos,
        IPasswordHasher<Usuario> hasher)
    {
        _context = context;
        _archivos = archivos;
        _hasher = hasher;
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
            query = query.Where(m =>
                m.Nombre.Contains(texto) ||
                m.Dueno!.Cedula.Contains(texto));
        }

        ViewBag.Q = q;

        var lista = await query
            .OrderBy(m => m.Nombre)
            .ToListAsync();

        return View(lista);
    }

    public async Task<IActionResult> Crear()
    {
        await CargarListasFormularioAsync();
        return View(new MascotaFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(MascotaFormViewModel mascotaViewModel)
    {
        mascotaViewModel.Nombre = (mascotaViewModel.Nombre ?? "").Trim();

        if (!Enum.IsDefined(typeof(Sexo), mascotaViewModel.Sexo))
        {
            ModelState.AddModelError(nameof(mascotaViewModel.Sexo), "Seleccione el sexo de la mascota.");
        }

        if (mascotaViewModel.FechaNacimiento.Date > DateTime.Today)
        {
            ModelState.AddModelError(nameof(mascotaViewModel.FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
        }

        if (mascotaViewModel.EspecieId > 0 && !await _context.Especies.AnyAsync(e => e.Id == mascotaViewModel.EspecieId))
        {
            ModelState.AddModelError(nameof(mascotaViewModel.EspecieId), "Seleccione una especie válida.");
        }

        if (mascotaViewModel.RazaId > 0 && !await _context.Razas.AnyAsync(r => r.Id == mascotaViewModel.RazaId && r.EspecieId == mascotaViewModel.EspecieId))
        {
            ModelState.AddModelError(nameof(mascotaViewModel.RazaId), "La raza seleccionada no pertenece a la especie indicada.");
        }

        var dueno = mascotaViewModel.DuenoId > 0
            ? await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == mascotaViewModel.DuenoId && u.Perfil == Perfil.Dueno)
            : null;

        if (dueno is null)
        {
            ModelState.AddModelError(nameof(mascotaViewModel.DuenoId), "Debe buscar y seleccionar un dueño válido o registrarlo.");
        }
        else
        {
            mascotaViewModel.DuenoCedula = dueno.Cedula;
            mascotaViewModel.DuenoNombre = dueno.Nombre;
        }

        if (!ModelState.IsValid)
        {
            await CargarListasFormularioAsync(mascotaViewModel.EspecieId, mascotaViewModel.RazaId);
            return View(mascotaViewModel);
        }

        var mascota = new Mascota
        {
            Nombre = mascotaViewModel.Nombre,
            EspecieId = mascotaViewModel.EspecieId,
            RazaId = mascotaViewModel.RazaId,
            FechaNacimiento = mascotaViewModel.FechaNacimiento.Date,
            Sexo = mascotaViewModel.Sexo,
            DuenoId = mascotaViewModel.DuenoId
        };

        try
        {
            if (mascotaViewModel.Foto is not null)
            {
                mascota.FotoRuta = await _archivos.GuardarFotoAsync(mascotaViewModel.Foto, "mascotas");
            }
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(nameof(mascotaViewModel.Foto), ex.Message);
            await CargarListasFormularioAsync(mascotaViewModel.EspecieId, mascotaViewModel.RazaId);
            return View(mascotaViewModel);
        }

        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Mascota registrada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> RazasPorEspecie(int especieId)
    {
        var razas = await _context.Razas
            .Where(r => r.EspecieId == especieId)
            .OrderBy(r => r.Nombre)
            .Select(r => new { r.Id, r.Nombre })
            .ToListAsync();

        return Json(razas);
    }

    [HttpGet]
    public async Task<IActionResult> BuscarDueno(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            return BadRequest(new { mensaje = "Ingrese la cédula para buscar." });
        }

        var texto = cedula.Trim();
        var dueno = await _context.Usuarios
            .Where(u => u.Cedula == texto && u.Perfil == Perfil.Dueno)
            .Select(u => new { u.Id, u.Cedula, u.Nombre, u.Correo })
            .FirstOrDefaultAsync();

        return dueno is null ? NotFound() : Json(dueno);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearDueno([FromBody] CrearDuenoViewModel duenoViewModel)
    {
        if (duenoViewModel is null)
        {
            return BadRequest(new { mensaje = "Datos inválidos." });
        }

        duenoViewModel.Cedula = (duenoViewModel.Cedula ?? "").Trim();
        duenoViewModel.Nombre = (duenoViewModel.Nombre ?? "").Trim();
        duenoViewModel.Correo = (duenoViewModel.Correo ?? "").Trim();

        if (!TryValidateModel(duenoViewModel))
        {
            var primerError = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Verifique los datos ingresados.";
            return BadRequest(new { mensaje = primerError });
        }

        if (await _context.Usuarios.AnyAsync(u => u.Cedula == duenoViewModel.Cedula))
        {
            return Conflict(new { mensaje = "Ya existe un usuario registrado con esa cédula." });
        }

        if (await _context.Usuarios.AnyAsync(u => u.Correo == duenoViewModel.Correo))
        {
            return Conflict(new { mensaje = "Ya existe un usuario registrado con ese correo electrónico." });
        }

        var dueno = new Usuario
        {
            Cedula = duenoViewModel.Cedula,
            Nombre = duenoViewModel.Nombre,
            Correo = duenoViewModel.Correo,
            Perfil = Perfil.Dueno,
            Bloqueado = false
        };
        dueno.ContrasenaHash = _hasher.HashPassword(dueno, duenoViewModel.Contrasena);

        _context.Usuarios.Add(dueno);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = dueno.Id,
            cedula = dueno.Cedula,
            nombre = dueno.Nombre,
            correo = dueno.Correo
        });
    }

    private async Task CargarListasFormularioAsync(int? especieId = null, int? razaId = null)
    {
        var especies = await _context.Especies
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        ViewBag.Especies = new SelectList(especies, nameof(Especie.Id), nameof(Especie.Nombre), especieId);

        var razas = especieId.HasValue && especieId.Value > 0
            ? await _context.Razas
                .Where(r => r.EspecieId == especieId.Value)
                .OrderBy(r => r.Nombre)
                .ToListAsync()
            : new List<Raza>();

        ViewBag.Razas = new SelectList(razas, nameof(Raza.Id), nameof(Raza.Nombre), razaId);
    }
}