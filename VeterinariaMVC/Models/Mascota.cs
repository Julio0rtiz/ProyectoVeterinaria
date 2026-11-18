using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Mascota
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(80, ErrorMessage = "El nombre no puede superar 80 caracteres.")]
    public string Nombre { get; set; } = "";

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la especie.")]
    [Display(Name = "Especie")]
    public int EspecieId { get; set; }
    public Especie? Especie { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione la raza.")]
    [Display(Name = "Raza")]
    public int RazaId { get; set; }
    public Raza? Raza { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de nacimiento")]
    public DateTime FechaNacimiento { get; set; }

    [EnumDataType(typeof(Sexo), ErrorMessage = "Seleccione el sexo.")]
    public Sexo Sexo { get; set; }

    [MaxLength(300)]
    [Display(Name = "Fotografía")]
    public string? FotoRuta { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el dueño.")]
    [Display(Name = "Dueño")]
    public int DuenoId { get; set; }
    public Usuario? Dueno { get; set; }

    [Display(Name = "Última atención")]
    public DateTime? UltimaAtencion { get; set; }
}
