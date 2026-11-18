using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Examen
{
    [Key]
    public int Id { get; set; }

    public int MascotaId { get; set; }
    public Mascota? Mascota { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(200, ErrorMessage = "La descripción no puede superar 200 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "";

    [Required]
    [MaxLength(300)]
    public string ArchivoRuta { get; set; } = "";

    [Required]
    [MaxLength(255)]
    [Display(Name = "Archivo")]
    public string NombreOriginal { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string TipoContenido { get; set; } = "";

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public int AutorId { get; set; }
    public Usuario? Autor { get; set; }
}
