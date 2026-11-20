using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.Models.ViewModels;

namespace VeterinariaMVC.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<Usuario> _hasher;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IPasswordHasher<Usuario> hasher, IConfiguration configuration)
    {
        _context = context;
        _hasher = hasher;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel credenciales)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == credenciales.Correo);

        if (usuario == null ||
            _hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, credenciales.Contrasena) == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
        }

        if (usuario.Bloqueado)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensaje = "Usuario bloqueado. Contacte a la clínica." });
        }

        var (token, expira) = GenerarToken(usuario);

        return Ok(new
        {
            token,
            expiraEn = expira,
            nombre = usuario.Nombre,
            perfil = usuario.Perfil.ToString()
        });
    }

    [HttpGet("sesion")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Sesion()
    {
        return Ok(new
        {
            id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            nombre = User.FindFirstValue(ClaimTypes.Name),
            perfil = User.FindFirstValue(ClaimTypes.Role)
        });
    }

    private (string token, DateTime expira) GenerarToken(Usuario usuario)
    {
        var jwt = _configuration.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireInMinutes"]!));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }

    [HttpPost("registro")]
    [AllowAnonymous]
    public async Task<IActionResult> Registro(RegistroViewModel datos)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Cedula == datos.Cedula || u.Correo == datos.Correo))
        {
            return Conflict(new { mensaje = "Ya existe una cuenta con esa cédula o correo." });
        }

        var usuario = new Usuario
        {
            Cedula = datos.Cedula.Trim(),
            Nombre = datos.Nombre.Trim(),
            Correo = datos.Correo.Trim().ToLower(),
            Perfil = Perfil.Dueno
        };

        usuario.ContrasenaHash = _hasher.HashPassword(usuario, datos.Contrasena);
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var (token, expira) = GenerarToken(usuario);
        return StatusCode(StatusCodes.Status201Created,
            new { token, expiraEn = expira, nombre = usuario.Nombre, perfil = usuario.Perfil.ToString() });
    }
}
