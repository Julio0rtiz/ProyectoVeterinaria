using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaMVC.Models;

public class TratamientoAsignado
{
    [Key]
    public int Id { get; set; }

    public int MascotaId { get; set; }
    public Mascota? Mascota { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un medicamento o tratamiento.")]
    [Display(Name = "Medicamento o tratamiento")]
    public int ItemCatalogoId { get; set; }
    public ItemCatalogo? ItemCatalogo { get; set; }

    [Required(ErrorMessage = "Las indicaciones son obligatorias.")]
    [MaxLength(500, ErrorMessage = "Las indicaciones no pueden superar 500 caracteres.")]
    public string Indicaciones { get; set; } = "";

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    public int AutorId { get; set; }
    public Usuario? Autor { get; set; }

    public bool Suspendido { get; set; }

    [Display(Name = "Fecha de suspensión")]
    public DateTime? FechaSuspension { get; set; }

    public int? AutorSuspensionId { get; set; }
    public Usuario? AutorSuspension { get; set; }

    [NotMapped]
    public bool Vigente => !Suspendido;
}
