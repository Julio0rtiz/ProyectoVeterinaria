namespace VeterinariaMVC.Models.ViewModels;

public class ExpedienteViewModel
{
    public Mascota Mascota { get; set; } = null!;
    public string Tab { get; set; } = "vacunas";
    public List<Vacuna> Vacunas { get; set; } = new();
    public List<TratamientoAsignado> Tratamientos { get; set; } = new();
    public List<Examen> Examenes { get; set; } = new();
    public List<NotaHistorial> Notas { get; set; } = new();
    public List<ItemCatalogo> Catalogo { get; set; } = new();
}
