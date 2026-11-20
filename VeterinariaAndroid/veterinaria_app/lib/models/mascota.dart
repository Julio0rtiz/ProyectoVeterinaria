class Mascota {
  final int id;
  final String nombre;
  final String especie;
  final String raza;
  final DateTime fechaNacimiento;
  final String sexo;
  final String? fotoUrl;

  Mascota({
    required this.id,
    required this.nombre,
    required this.especie,
    required this.raza,
    required this.fechaNacimiento,
    required this.sexo,
    this.fotoUrl,
  });

  factory Mascota.fromJson(Map<String, dynamic> json) {
    return Mascota(
      id: json['id'] as int,
      nombre: json['nombre'] as String,
      especie: json['especie'] as String,
      raza: json['raza'] as String,
      fechaNacimiento: DateTime.parse(json['fechaNacimiento'] as String),
      sexo: json['sexo'] as String,
      fotoUrl: json['fotoUrl'] as String?,
    );
  }
}