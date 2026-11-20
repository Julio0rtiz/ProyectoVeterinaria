class Tratamiento {
  final int id;
  final String nombre;
  final String tipo;
  final String indicaciones;
  final DateTime fechaInicio;
  final DateTime? fechaSuspension;
  final bool suspendido;
  final String veterinario;

  Tratamiento({
    required this.id,
    required this.nombre,
    required this.tipo,
    required this.indicaciones,
    required this.fechaInicio,
    this.fechaSuspension,
    required this.suspendido,
    required this.veterinario,
  });

  factory Tratamiento.fromJson(Map<String, dynamic> json) {
    return Tratamiento(
      id: json['id'] as int,
      nombre: json['nombre'] as String,
      tipo: json['tipo'] as String,
      indicaciones: json['indicaciones'] as String,
      fechaInicio: DateTime.parse(json['fechaInicio'] as String),
      fechaSuspension: json['fechaSuspension'] != null
          ? DateTime.parse(json['fechaSuspension'] as String)
          : null,
      suspendido: json['suspendido'] as bool,
      veterinario: json['veterinario'] as String,
    );
  }
}

class TratamientosResponse {
  final List<Tratamiento> vigentes;
  final List<Tratamiento> suspendidos;

  TratamientosResponse({
    required this.vigentes,
    required this.suspendidos,
  });

  factory TratamientosResponse.fromJson(Map<String, dynamic> json) {
    return TratamientosResponse(
      vigentes: (json['vigentes'] as List<dynamic>)
          .map((e) => Tratamiento.fromJson(e as Map<String, dynamic>))
          .toList(),
      suspendidos: (json['suspendidos'] as List<dynamic>)
          .map((e) => Tratamiento.fromJson(e as Map<String, dynamic>))
          .toList(),
    );
  }
}