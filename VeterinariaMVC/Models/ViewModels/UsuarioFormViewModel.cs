using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models.ViewModels;

public class UsuarioFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [MaxLength(20, ErrorMessage = "La cédula no puede superar 20 caracteres.")]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = "";

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [MaxLength(150, ErrorMessage = "El correo no puede superar 150 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = "";

    [DataType(DataType.Password)]
    [MaxLength(100, ErrorMessage = "La contraseña no puede superar 100 caracteres.")]
    [Display(Name = "Contraseña")]
    public string? Contrasena { get; set; }

    [EnumDataType(typeof(Perfil), ErrorMessage = "Seleccione un perfil válido.")]
    public Perfil Perfil { get; set; }

    [Display(Name = "Veterinario vinculado")]
    public int? VeterinarioId { get; set; }
}