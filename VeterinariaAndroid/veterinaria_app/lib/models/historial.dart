class NotaHistorial {
  final DateTime fechaHora;
  final String contenidoHtml;
  final String veterinario;

  NotaHistorial({
    required this.fechaHora,
    required this.contenidoHtml,
    required this.veterinario,
  });

  factory NotaHistorial.fromJson(Map<String, dynamic> json) {
    return NotaHistorial(
      fechaHora: DateTime.parse(json['fechaHora'] as String),
      contenidoHtml: json['contenidoHtml'] as String,
      veterinario: json['veterinario'] as String,
    );
  }
}