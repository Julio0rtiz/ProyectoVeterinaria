using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class ItemCatalogo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = "";

    [EnumDataType(typeof(TipoItemCatalogo), ErrorMessage = "Seleccione un tipo válido.")]
    public TipoItemCatalogo Tipo { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "";

    public ICollection<TratamientoAsignado> Asignaciones { get; set; } = new List<TratamientoAsignado>();
}
