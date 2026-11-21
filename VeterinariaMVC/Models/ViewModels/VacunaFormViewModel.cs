using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models.ViewModels;

public class VacunaFormViewModel
{
    [Required(ErrorMessage = "El nombre de la vacuna es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    [Display(Name = "Vacuna")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage = "La fecha de aplicación es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de aplicación")]
    public DateTime? FechaAplicacion { get; set; }

    [Required(ErrorMessage = "La fecha de la próxima dosis es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Próxima dosis")]
    public DateTime? FechaProximaDosis { get; set; }
}
