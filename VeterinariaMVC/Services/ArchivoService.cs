namespace VeterinariaMVC.Services;

public class ArchivoService : IArchivoService
{
    private const long MaximoFoto = 5 * 1024 * 1024;
    private const long MaximoPrivado = 10 * 1024 * 1024;

    private static readonly string[] ExtensionesFoto = { ".jpg", ".jpeg", ".png" };
    private static readonly string[] ExtensionesPrivado = { ".jpg", ".jpeg", ".png", ".pdf" };

    private readonly IWebHostEnvironment _env;

    public ArchivoService(IWebHostEnvironment env)
    {
        _env = env;
    }

    private string CarpetaPublica => _env.WebRootPath;
    private string CarpetaPrivada => Path.Combine(_env.ContentRootPath, "App_Data", "archivos");

    public async Task<string> GuardarFotoAsync(IFormFile archivo, string carpeta)
    {
        var extension = Validar(archivo, ExtensionesFoto, MaximoFoto,
            "Solo se permiten imágenes JPG o PNG.", "La imagen no puede superar 5 MB.");

        var nombre = await GuardarAsync(archivo, Path.Combine(CarpetaPublica, "uploads", carpeta), extension);
        return $"uploads/{carpeta}/{nombre}";
    }

    public async Task<string> GuardarPrivadoAsync(IFormFile archivo, string carpeta)
    {
        var extension = Validar(archivo, ExtensionesPrivado, MaximoPrivado,
            "Solo se permiten archivos JPG, PNG o PDF.", "El archivo no puede superar 10 MB.");

        var nombre = await GuardarAsync(archivo, Path.Combine(CarpetaPrivada, carpeta), extension);
        return $"{carpeta}/{nombre}";
    }

    public (string rutaFisica, string tipoContenido) ObtenerPrivado(string rutaRelativa)
    {
        var rutaFisica = RutaSegura(CarpetaPrivada, rutaRelativa);

        if (!File.Exists(rutaFisica))
        {
            throw new FileNotFoundException("El archivo no existe.");
        }

        return (rutaFisica, TipoContenido(Path.GetExtension(rutaFisica)));
    }

    public void Eliminar(string rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa))
        {
            return;
        }

        var rutaFisica = rutaRelativa.StartsWith("uploads/")
            ? RutaSegura(CarpetaPublica, rutaRelativa)
            : RutaSegura(CarpetaPrivada, rutaRelativa);

        if (File.Exists(rutaFisica))
        {
            File.Delete(rutaFisica);
        }
    }

    private static string Validar(IFormFile archivo, string[] permitidas, long maximo,
        string mensajeExtension, string mensajeTamano)
    {
        if (archivo == null || archivo.Length == 0)
        {
            throw new ArgumentException("Seleccione un archivo.");
        }

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        if (!permitidas.Contains(extension))
        {
            throw new ArgumentException(mensajeExtension);
        }

        if (archivo.Length > maximo)
        {
            throw new ArgumentException(mensajeTamano);
        }

        return extension;
    }

    private static async Task<string> GuardarAsync(IFormFile archivo, string directorio, string extension)
    {
        Directory.CreateDirectory(directorio);

        var nombre = $"{Guid.NewGuid()}{extension}";

        await using var stream = File.Create(Path.Combine(directorio, nombre));
        await archivo.CopyToAsync(stream);

        return nombre;
    }

    private static string RutaSegura(string carpetaBase, string rutaRelativa)
    {
        var baseCompleta = Path.GetFullPath(carpetaBase) + Path.DirectorySeparatorChar;
        var rutaCompleta = Path.GetFullPath(Path.Combine(carpetaBase, rutaRelativa));

        if (!rutaCompleta.StartsWith(baseCompleta, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Ruta de archivo no válida.");
        }

        return rutaCompleta;
    }

    private static string TipoContenido(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
