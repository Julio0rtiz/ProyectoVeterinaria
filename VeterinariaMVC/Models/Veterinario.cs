using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Veterinario
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre completo no puede superar 100 caracteres.")]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = "";

    [Required(ErrorMessage = "El número de colegiado es obligatorio.")]
    [MaxLength(20, ErrorMessage = "El número de colegiado no puede superar 20 caracteres.")]
    [Display(Name = "Número de colegiado")]
    public string NumeroColegiado { get; set; } = "";

    [MaxLength(300)]
    [Display(Name = "Fotografía")]
    public string? FotoRuta { get; set; }
}
