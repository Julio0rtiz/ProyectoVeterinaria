using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class NotaHistorial
{
    [Key]
    public int Id { get; set; }

    public int MascotaId { get; set; }
    public Mascota? Mascota { get; set; }

    [Required(ErrorMessage = "El contenido de la nota es obligatorio.")]
    [Display(Name = "Nota")]
    public string ContenidoHtml { get; set; } = "";

    [Display(Name = "Fecha y hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    public int AutorId { get; set; }
    public Usuario? Autor { get; set; }
}
