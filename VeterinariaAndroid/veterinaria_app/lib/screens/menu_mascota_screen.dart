import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/mascota.dart';
import 'examenes_screen.dart';
import 'historial_screen.dart';
import 'tratamientos_screen.dart';
import 'vacunas_screen.dart';

class MenuMascotaScreen extends StatelessWidget {
  final Mascota mascota;

  const MenuMascotaScreen({super.key, required this.mascota});

  @override
  Widget build(BuildContext context) {
    final fechaNac = DateFormat('dd/MM/yyyy').format(mascota.fechaNacimiento);

    return Scaffold(
      appBar: AppBar(
        title: Text(mascota.nombre),
      ),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Card(
            clipBehavior: Clip.antiAlias,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Container(
                  height: 200,
                  color: const Color(0xFFE6F4F1),
                  child: mascota.fotoUrl != null && mascota.fotoUrl!.isNotEmpty
                      ? Image.network(
                          mascota.fotoUrl!,
                          fit: BoxFit.cover,
                          errorBuilder: (_, __, ___) => const Icon(
                            Icons.pets,
                            size: 72,
                            color: Color(0xFF0F766E),
                          ),
                        )
                      : const Icon(
                          Icons.pets,
                          size: 72,
                          color: Color(0xFF0F766E),
                        ),
                ),
                Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        mascota.nombre,
                        style: const TextStyle(
                          fontSize: 22,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF1E293B),
                        ),
                      ),
                      const SizedBox(height: 6),
                      Text(
                        '${mascota.especie} • ${mascota.raza}',
                        style: const TextStyle(
                          fontSize: 15,
                          color: Color(0xFF475569),
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'Sexo: ${mascota.sexo} • Nacimiento: $fechaNac',
                        style: const TextStyle(
                          fontSize: 13,
                          color: Color(0xFF64748B),
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 16),
          const Padding(
            padding: EdgeInsets.symmetric(horizontal: 4, vertical: 6),
            child: Text(
              'EXPEDIENTE CLÍNICO',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                letterSpacing: 0.8,
                color: Color(0xFF64748B),
              ),
            ),
          ),
          _OpcionExpediente(
            icono: Icons.vaccines_outlined,
            titulo: 'Ver vacunas',
            subtitulo: 'Historial de vacunación y próximas dosis',
            onTap: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => VacunasScreen(mascota: mascota),
                ),
              );
            },
          ),
          const SizedBox(height: 10),
          _OpcionExpediente(
            icono: Icons.description_outlined,
            titulo: 'Ver resultados de exámenes',
            subtitulo: 'Archivos de laboratorio e imágenes diagnósticas',
            onTap: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => ExamenesScreen(mascota: mascota),
                ),
              );
            },
          ),
          const SizedBox(height: 10),
          _OpcionExpediente(
            icono: Icons.medication_outlined,
            titulo: 'Ver tratamientos',
            subtitulo: 'Medicamentos y tratamientos vigentes o suspendidos',
            onTap: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => TratamientosScreen(mascota: mascota),
                ),
              );
            },
          ),
          const SizedBox(height: 10),
          _OpcionExpediente(
            icono: Icons.history_edu_outlined,
            titulo: 'Ver historial clínico',
            subtitulo: 'Notas de evolución registradas por el veterinario',
            onTap: () {
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (_) => HistorialScreen(mascota: mascota),
                ),
              );
            },
          ),
        ],
      ),
    );
  }
}

class _OpcionExpediente extends StatelessWidget {
  final IconData icono;
  final String titulo;
  final String subtitulo;
  final VoidCallback onTap;

  const _OpcionExpediente({
    required this.icono,
    required this.titulo,
    required this.subtitulo,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        leading: CircleAvatar(
          backgroundColor: const Color(0xFFE6F4F1),
          foregroundColor: const Color(0xFF0F766E),
          child: Icon(icono),
        ),
        title: Text(
          titulo,
          style: const TextStyle(
            fontWeight: FontWeight.bold,
            color: Color(0xFF1E293B),
          ),
        ),
        subtitle: Text(
          subtitulo,
          style: const TextStyle(fontSize: 13),
        ),
        trailing: const Icon(Icons.chevron_right),
        onTap: onTap,
      ),
    );
  }
}