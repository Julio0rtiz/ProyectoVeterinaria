namespace VeterinariaMVC.Services;

public interface IArchivoService
{
    Task<string> GuardarFotoAsync(IFormFile archivo, string carpeta);
    Task<string> GuardarPrivadoAsync(IFormFile archivo, string carpeta);
    (string rutaFisica, string tipoContenido) ObtenerPrivado(string rutaRelativa);
    void Eliminar(string rutaRelativa);
}
