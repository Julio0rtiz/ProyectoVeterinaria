using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Raza
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El nombre no puede superar 50 caracteres.")]
    public string Nombre { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la especie.")]
    [Display(Name = "Especie")]
    public int EspecieId { get; set; }
    public Especie? Especie { get; set; }

    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}
