class Vacuna {
  final int id;
  final String nombre;
  final DateTime fechaAplicacion;
  final DateTime fechaProximaDosis;
  final bool vencida;
  final String veterinario;

  Vacuna({
    required this.id,
    required this.nombre,
    required this.fechaAplicacion,
    required this.fechaProximaDosis,
    required this.vencida,
    required this.veterinario,
  });

  factory Vacuna.fromJson(Map<String, dynamic> json) {
    return Vacuna(
      id: json['id'] as int,
      nombre: json['nombre'] as String,
      fechaAplicacion: DateTime.parse(json['fechaAplicacion'] as String),
      fechaProximaDosis: DateTime.parse(json['fechaProximaDosis'] as String),
      vencida: json['vencida'] as bool,
      veterinario: json['veterinario'] as String,
    );
  }
}