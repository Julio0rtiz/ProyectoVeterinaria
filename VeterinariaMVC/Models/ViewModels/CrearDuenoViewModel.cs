using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models.ViewModels;

public class CrearDuenoViewModel
{
    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [MaxLength(20, ErrorMessage = "La cédula no puede superar 20 caracteres.")]
    public string Cedula { get; set; } = "";

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [MaxLength(150, ErrorMessage = "El correo no puede superar 150 caracteres.")]
    public string Correo { get; set; } = "";

    [Required(ErrorMessage = "La contraseña inicial es obligatoria.")]
    [MaxLength(100, ErrorMessage = "La contraseña no puede superar 100 caracteres.")]
    public string Contrasena { get; set; } = "";
}