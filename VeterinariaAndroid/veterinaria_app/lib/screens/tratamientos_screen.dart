import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/mascota.dart';
import '../models/tratamiento.dart';
import '../services/api_client.dart';
import '../services/auth_service.dart';
import '../services/veterinaria_api.dart';
import '../widgets/error_view.dart';
import 'login_screen.dart';

class TratamientosScreen extends StatefulWidget {
  final Mascota mascota;

  const TratamientosScreen({super.key, required this.mascota});

  @override
  State<TratamientosScreen> createState() => _TratamientosScreenState();
}

class _TratamientosScreenState extends State<TratamientosScreen> {
  final _auth = AuthService();
  late final VeterinariaApi _api;
  TratamientosResponse? _datos;
  String? _error;
  bool _cargando = true;

  @override
  void initState() {
    super.initState();
    _api = VeterinariaApi(ApiClient(_auth));
    _cargar();
  }

  Future<void> _cargar() async {
    setState(() {
      _cargando = true;
      _error = null;
    });

    try {
      final res = await _api.tratamientos(widget.mascota.id);
      if (!mounted) return;
      setState(() {
        _datos = res;
        _cargando = false;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      if (e.sesionExpirada) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.mensaje)));
        Navigator.pushAndRemoveUntil(
          context,
          MaterialPageRoute(builder: (_) => const LoginScreen()),
          (route) => false,
        );
        return;
      }
      setState(() {
        _error = e.mensaje;
        _cargando = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _error = 'No se pudieron cargar los tratamientos.';
        _cargando = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Tratamientos • ${widget.mascota.nombre}'),
      ),
      body: _construirCuerpo(),
    );
  }

  Widget _construirCuerpo() {
    if (_cargando) return const Center(child: CircularProgressIndicator());
    if (_error != null) return ErrorView(mensaje: _error!, onReintentar: _cargar);

    final vigentes = _datos?.vigentes ?? [];
    final suspendidos = _datos?.suspendidos ?? [];

    if (vigentes.isEmpty && suspendidos.isEmpty) {
      return const Center(
        child: Text(
          'No hay tratamientos registrados para esta mascota.',
          style: TextStyle(fontSize: 15, color: Color(0xFF475569)),
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _cargar,
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (vigentes.isNotEmpty) ...[
            const Text(
              'VIGENTES',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                letterSpacing: 0.8,
                color: Color(0xFF0F766E),
              ),
            ),
            const SizedBox(height: 8),
            ...vigentes.map((t) => Padding(
                  padding: const EdgeInsets.only(bottom: 10),
                  child: _TarjetaTratamiento(tratamiento: t),
                )),
            const SizedBox(height: 12),
          ],
          if (suspendidos.isNotEmpty) ...[
            const Text(
              'SUSPENDIDOS',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.bold,
                letterSpacing: 0.8,
                color: Color(0xFF64748B),
              ),
            ),
            const SizedBox(height: 8),
            ...suspendidos.map((t) => Padding(
                  padding: const EdgeInsets.only(bottom: 10),
                  child: _TarjetaTratamiento(tratamiento: t),
                )),
          ],
        ],
      ),
    );
  }
}

class _TarjetaTratamiento extends StatelessWidget {
  final Tratamiento tratamiento;

  const _TarjetaTratamiento({required this.tratamiento});

  @override
  Widget build(BuildContext context) {
    final formato = DateFormat('dd/MM/yyyy');

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    tratamiento.nombre,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFF1E293B),
                    ),
                  ),
                ),
                Chip(
                  label: Text(
                    tratamiento.tipo,
                    style: const TextStyle(fontSize: 12),
                  ),
                  visualDensity: VisualDensity.compact,
                ),
              ],
            ),
            const SizedBox(height: 6),
            Text(
              tratamiento.indicaciones,
              style: const TextStyle(fontSize: 14, color: Color(0xFF334155)),
            ),
            const SizedBox(height: 8),
            Text(
              'Inicio: ${formato.format(tratamiento.fechaInicio)}'
              '${tratamiento.fechaSuspension != null ? ' • Suspendido: ${formato.format(tratamiento.fechaSuspension!)}' : ''}',
              style: const TextStyle(fontSize: 13, color: Color(0xFF64748B)),
            ),
            Text(
              'Veterinario: ${tratamiento.veterinario}',
              style: const TextStyle(fontSize: 13, color: Color(0xFF64748B)),
            ),
          ],
        ),
      ),
    );
  }
}