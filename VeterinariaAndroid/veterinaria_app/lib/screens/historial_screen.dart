import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../models/historial.dart';
import '../models/mascota.dart';
import '../services/api_client.dart';
import '../services/auth_service.dart';
import '../services/veterinaria_api.dart';
import '../widgets/error_view.dart';
import 'login_screen.dart';

class HistorialScreen extends StatefulWidget {
  final Mascota mascota;

  const HistorialScreen({super.key, required this.mascota});

  @override
  State<HistorialScreen> createState() => _HistorialScreenState();
}

class _HistorialScreenState extends State<HistorialScreen> {
  final _auth = AuthService();
  late final VeterinariaApi _api;
  List<NotaHistorial>? _notas;
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
      final datos = await _api.historial(widget.mascota.id);
      if (!mounted) return;
      setState(() {
        _notas = datos;
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
        _error = 'No se pudo cargar el historial clínico.';
        _cargando = false;
      });
    }
  }

  String _limpiarHtml(String html) {
    return html
        .replaceAll(RegExp(r'<br\s*/?>', caseSensitive: false), '\n')
        .replaceAll(RegExp(r'</p>', caseSensitive: false), '\n')
        .replaceAll(RegExp(r'<[^>]*>'), '')
        .replaceAll('&nbsp;', ' ')
        .trim();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Historial • ${widget.mascota.nombre}'),
      ),
      body: _construirCuerpo(),
    );
  }

  Widget _construirCuerpo() {
    if (_cargando) return const Center(child: CircularProgressIndicator());
    if (_error != null) return ErrorView(mensaje: _error!, onReintentar: _cargar);

    final lista = _notas ?? [];
    if (lista.isEmpty) {
      return const Center(
        child: Text(
          'No hay notas clínicas registradas para esta mascota.',
          style: TextStyle(fontSize: 15, color: Color(0xFF475569)),
        ),
      );
    }

    final formato = DateFormat('dd/MM/yyyy HH:mm');

    return RefreshIndicator(
      onRefresh: _cargar,
      child: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: lista.length,
        separatorBuilder: (_, __) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          final nota = lista[index];
          return Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        formato.format(nota.fechaHora),
                        style: const TextStyle(
                          fontSize: 13,
                          fontWeight: FontWeight.bold,
                          color: Color(0xFF0F766E),
                        ),
                      ),
                      Text(
                        nota.veterinario,
                        style: const TextStyle(
                          fontSize: 13,
                          color: Color(0xFF64748B),
                        ),
                      ),
                    ],
                  ),
                  const Divider(height: 20),
                  Text(
                    _limpiarHtml(nota.contenidoHtml),
                    style: const TextStyle(
                      fontSize: 14,
                      height: 1.4,
                      color: Color(0xFF1E293B),
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}