using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models.ViewModels;

public class ExamenFormViewModel
{
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(200, ErrorMessage = "La descripción no puede superar 200 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "";

    [Required(ErrorMessage = "Seleccione un archivo.")]
    [Display(Name = "Archivo")]
    public IFormFile? Archivo { get; set; }
}
