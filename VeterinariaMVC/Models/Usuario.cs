using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Usuario
{
    [Key]
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

    [Required]
    public string ContrasenaHash { get; set; } = "";

    [EnumDataType(typeof(Perfil), ErrorMessage = "Seleccione un perfil válido.")]
    public Perfil Perfil { get; set; }

    public bool Bloqueado { get; set; }

    [Display(Name = "Veterinario")]
    public int? VeterinarioId { get; set; }
    public Veterinario? Veterinario { get; set; }

    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}
