using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models.ViewModels;

public class TratamientoFormViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un medicamento o tratamiento.")]
    [Display(Name = "Medicamento o tratamiento")]
    public int ItemCatalogoId { get; set; }

    [Required(ErrorMessage = "Las indicaciones son obligatorias.")]
    [MaxLength(500, ErrorMessage = "Las indicaciones no pueden superar 500 caracteres.")]
    public string Indicaciones { get; set; } = "";

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime? FechaInicio { get; set; }
}
