using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Especie
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
    public string Nombre { get; set; } = "";
}
