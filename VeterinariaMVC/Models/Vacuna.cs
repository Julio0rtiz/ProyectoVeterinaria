using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Vacuna
{
    [Key]
    public int Id { get; set; }

    public int MascotaId { get; set; }
    public Mascota? Mascota { get; set; }

    [Required(ErrorMessage = "El nombre de la vacuna es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    [Display(Name = "Vacuna")]
    public string Nombre { get; set; } = "";

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de aplicación")]
    public DateTime FechaAplicacion { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Próxima dosis")]
    public DateTime FechaProximaDosis { get; set; }

    public int AutorId { get; set; }
    public Usuario? Autor { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
