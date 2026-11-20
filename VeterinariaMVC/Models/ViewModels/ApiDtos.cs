namespace VeterinariaMVC.Models.ViewModels;

public record MascotaDto(
    int Id,
    string Nombre,
    string Especie,
    string Raza,
    DateTime FechaNacimiento,
    string Sexo,
    string? FotoUrl);

public record VacunaDto(
    int Id,
    string Nombre,
    DateTime FechaAplicacion,
    DateTime FechaProximaDosis,
    bool Vencida,
    string Veterinario);

public record ExamenDto(
    int Id,
    string Descripcion,
    DateTime Fecha,
    string NombreArchivo,
    string TipoContenido,
    string Veterinario);

    public record TratamientoDto(
    int Id,
    string Nombre,
    string Tipo,
    string Indicaciones,
    DateTime FechaInicio,
    DateTime? FechaSuspension,
    bool Suspendido,
    string Veterinario);

public record HistorialDto(
    DateTime FechaHora,
    string ContenidoHtml,
    string Veterinario);