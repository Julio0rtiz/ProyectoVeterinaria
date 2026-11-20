class Examen {
  final int id;
  final String descripcion;
  final DateTime fecha;
  final String nombreArchivo;
  final String tipoContenido;
  final String veterinario;

  Examen({
    required this.id,
    required this.descripcion,
    required this.fecha,
    required this.nombreArchivo,
    required this.tipoContenido,
    required this.veterinario,
  });

  factory Examen.fromJson(Map<String, dynamic> json) {
    return Examen(
      id: json['id'] as int,
      descripcion: json['descripcion'] as String,
      fecha: DateTime.parse(json['fecha'] as String),
      nombreArchivo: json['nombreArchivo'] as String,
      tipoContenido: json['tipoContenido'] as String,
      veterinario: json['veterinario'] as String,
    );
  }
}