using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models.ViewModels;

public class MascotaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(80, ErrorMessage = "El nombre no puede superar 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la especie.")]
    [Display(Name = "Especie")]
    public int EspecieId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la raza.")]
    [Display(Name = "Raza")]
    public int RazaId { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime FechaNacimiento { get; set; } = DateTime.Today;

    [EnumDataType(typeof(Sexo), ErrorMessage = "Seleccione el sexo.")]
    public Sexo Sexo { get; set; }

    [Display(Name = "Fotografía")]
    public IFormFile? Foto { get; set; }

    public string? FotoActual { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar o registrar un dueño.")]
    [Display(Name = "Dueño")]
    public int DuenoId { get; set; }

    public string? DuenoCedula { get; set; }
    public string? DuenoNombre { get; set; }
}