using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.Models.ViewModels;

namespace VeterinariaMVC.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController : Controller
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _hasher;

    public UsuariosController(AppDbContext context, IPasswordHasher<Usuario> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<IActionResult> Index(Perfil? perfil, bool? bloqueado, string? q)
    {
        var query = _context.Usuarios
            .Include(u => u.Veterinario)
            .AsQueryable();

        if (perfil.HasValue && Enum.IsDefined(typeof(Perfil), perfil.Value))
        {
            query = query.Where(u => u.Perfil == perfil.Value);
        }

        if (bloqueado.HasValue)
        {
            query = query.Where(u => u.Bloqueado == bloqueado.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var texto = q.Trim();
            query = query.Where(u =>
                u.Nombre.Contains(texto) ||
                u.Cedula.Contains(texto) ||
                u.Correo.Contains(texto));
        }

        ViewBag.Perfil = perfil;
        ViewBag.Bloqueado = bloqueado;
        ViewBag.Q = q;
        ViewBag.UsuarioActualId = ObtenerUsuarioActualId();

        var lista = await query
            .OrderBy(u => u.Perfil)
            .ThenBy(u => u.Nombre)
            .ToListAsync();

        return View(lista);
    }

    public async Task<IActionResult> Crear()
    {
        await CargarVeterinariosDisponiblesAsync();
        return View(new UsuarioFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(UsuarioFormViewModel usuarioViewModel)
    {
        usuarioViewModel.Cedula = (usuarioViewModel.Cedula ?? "").Trim();
        usuarioViewModel.Nombre = (usuarioViewModel.Nombre ?? "").Trim();
        usuarioViewModel.Correo = (usuarioViewModel.Correo ?? "").Trim();

        if (!Enum.IsDefined(typeof(Perfil), usuarioViewModel.Perfil))
        {
            ModelState.AddModelError(nameof(usuarioViewModel.Perfil), "Seleccione un perfil válido.");
        }

        if (string.IsNullOrWhiteSpace(usuarioViewModel.Contrasena))
        {
            ModelState.AddModelError(nameof(usuarioViewModel.Contrasena), "La contraseña es obligatoria.");
        }

        if (await _context.Usuarios.AnyAsync(u => u.Cedula == usuarioViewModel.Cedula))
        {
            ModelState.AddModelError(nameof(usuarioViewModel.Cedula), "Ya existe un usuario con esa cédula.");
        }

        if (await _context.Usuarios.AnyAsync(u => u.Correo == usuarioViewModel.Correo))
        {
            ModelState.AddModelError(nameof(usuarioViewModel.Correo), "Ya existe un usuario con ese correo electrónico.");
        }

        if (usuarioViewModel.Perfil == Perfil.Veterinario)
        {
            if (!usuarioViewModel.VeterinarioId.HasValue || usuarioViewModel.VeterinarioId.Value <= 0)
            {
                ModelState.AddModelError(nameof(usuarioViewModel.VeterinarioId), "Debe seleccionar el veterinario vinculado.");
            }
            else if (!await _context.Veterinarios.AnyAsync(v => v.Id == usuarioViewModel.VeterinarioId.Value))
            {
                ModelState.AddModelError(nameof(usuarioViewModel.VeterinarioId), "Seleccione un veterinario válido.");
            }
            else if (await _context.Usuarios.AnyAsync(u => u.VeterinarioId == usuarioViewModel.VeterinarioId.Value))
            {
                ModelState.AddModelError(nameof(usuarioViewModel.VeterinarioId), "El veterinario seleccionado ya tiene un usuario vinculado.");
            }
        }
        else
        {
            usuarioViewModel.VeterinarioId = null;
        }

        if (!ModelState.IsValid)
        {
            await CargarVeterinariosDisponiblesAsync(usuarioViewModel.VeterinarioId);
            return View(usuarioViewModel);
        }

        var usuario = new Usuario
        {
            Cedula = usuarioViewModel.Cedula,
            Nombre = usuarioViewModel.Nombre,
            Correo = usuarioViewModel.Correo,
            Perfil = usuarioViewModel.Perfil,
            VeterinarioId = usuarioViewModel.Perfil == Perfil.Veterinario ? usuarioViewModel.VeterinarioId : null,
            Bloqueado = false
        };
        usuario.ContrasenaHash = _hasher.HashPassword(usuario, usuarioViewModel.Contrasena!);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Usuario agregado.";
        return RedirectToAction(nameof(Index));
    }

    private int ObtenerUsuarioActualId()
    {
        var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(valor, out var id) ? id : 0;
    }

    private async Task CargarVeterinariosDisponiblesAsync(int? seleccionado = null, int? usuarioId = null)
    {
        var ocupados = await _context.Usuarios
            .Where(u => u.VeterinarioId != null && (usuarioId == null || u.Id != usuarioId.Value))
            .Select(u => u.VeterinarioId!.Value)
            .ToListAsync();

        var disponibles = await _context.Veterinarios
            .Where(v => !ocupados.Contains(v.Id))
            .OrderBy(v => v.NombreCompleto)
            .Select(v => new
            {
                v.Id,
                Texto = $"{v.NombreCompleto} ({v.NumeroColegiado})"
            })
            .ToListAsync();

        ViewBag.Veterinarios = new SelectList(disponibles, "Id", "Texto", seleccionado);
    }
}