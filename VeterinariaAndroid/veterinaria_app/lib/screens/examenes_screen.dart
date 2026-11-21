import 'dart:io';
import 'package:material_ui/material_ui.dart';
import 'package:intl/intl.dart';
import 'package:open_filex/open_filex.dart';
import 'package:path_provider/path_provider.dart';
import '../models/examen.dart';
import '../models/mascota.dart';
import '../services/api_client.dart';
import '../services/auth_service.dart';
import '../services/veterinaria_api.dart';
import '../widgets/error_view.dart';
import 'login_screen.dart';

class ExamenesScreen extends StatefulWidget {
  final Mascota mascota;

  const ExamenesScreen({super.key, required this.mascota});

  @override
  State<ExamenesScreen> createState() => _ExamenesScreenState();
}

class _ExamenesScreenState extends State<ExamenesScreen> {
  final _auth = AuthService();
  late final VeterinariaApi _api;
  List<Examen>? _examenes;
  String? _error;
  bool _cargando = true;
  int? _descargandoId;

  @override
  void initState() {
    super.initState();
    _api = VeterinariaApi(ApiClient(_auth));
    _cargarExamenes();
  }

  Future<void> _cargarExamenes() async {
    setState(() {
      _cargando = true;
      _error = null;
    });

    try {
      final datos = await _api.examenes(widget.mascota.id);
      if (!mounted) return;
      setState(() {
        _examenes = datos;
        _cargando = false;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      if (e.sesionExpirada) {
        _irALogin(e.mensaje);
        return;
      }
      setState(() {
        _error = e.mensaje;
        _cargando = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _error = 'No se pudieron cargar los exámenes.';
        _cargando = false;
      });
    }
  }

  void _irALogin(String mensaje) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(mensaje)));
    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (_) => const LoginScreen()),
      (route) => false,
    );
  }

  Future<void> _abrirExamen(Examen ex) async {
    if (_descargandoId != null) return;

    setState(() => _descargandoId = ex.id);

    try {
      final bytes = await _api.descargarExamen(ex.id);
      final dir = await getApplicationDocumentsDirectory();
      final archivo = File('${dir.path}/${ex.id}_${ex.nombreArchivo}');
      await archivo.writeAsBytes(bytes);
      final res = await OpenFilex.open(archivo.path);
      if (res.type != ResultType.done) {
        throw ApiException('No hay una aplicación para abrir este archivo.');
      }
    } on ApiException catch (e) {
      if (!mounted) return;
      if (e.sesionExpirada) {
        _irALogin(e.mensaje);
        return;
      }
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(e.mensaje)),
      );
    } catch (_) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('No se pudo abrir el archivo del examen.')),
      );
    } finally {
      if (mounted) {
        setState(() => _descargandoId = null);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Exámenes • ${widget.mascota.nombre}'),
      ),
      body: _construirCuerpo(),
    );
  }

  Widget _construirCuerpo() {
    if (_cargando) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return ErrorView(mensaje: _error!, onReintentar: _cargarExamenes);
    }

    final lista = _examenes ?? [];
    if (lista.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Text(
            'No hay exámenes registrados para esta mascota.',
            textAlign: TextAlign.center,
            style: TextStyle(fontSize: 15, color: Color(0xFF475569)),
          ),
        ),
      );
    }

    final formato = DateFormat('dd/MM/yyyy');

    return RefreshIndicator(
      onRefresh: _cargarExamenes,
      child: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: lista.length,
        separatorBuilder: (_, _) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          final ex = lista[index];
          final esPdf = ex.tipoContenido.toLowerCase().contains('pdf');
          final esImagen = ex.tipoContenido.toLowerCase().contains('image');
          final descargando = _descargandoId == ex.id;

          return Card(
            child: ListTile(
              contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
              leading: CircleAvatar(
                backgroundColor: esPdf ? const Color(0xFFFEF2F2) : const Color(0xFFE6F4F1),
                foregroundColor: esPdf ? const Color(0xFFDC2626) : const Color(0xFF0F766E),
                child: Icon(
                  esPdf
                      ? Icons.picture_as_pdf_outlined
                      : (esImagen ? Icons.image_outlined : Icons.insert_drive_file_outlined),
                ),
              ),
              title: Text(
                ex.descripcion,
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  color: Color(0xFF1E293B),
                ),
              ),
              subtitle: Padding(
                padding: const EdgeInsets.only(top: 4),
                child: Text(
                  '${formato.format(ex.fecha)} • ${ex.veterinario}\n${ex.nombreArchivo}',
                  style: const TextStyle(fontSize: 13),
                ),
              ),
              trailing: descargando
                  ? const SizedBox(
                      width: 24,
                      height: 24,
                      child: CircularProgressIndicator(strokeWidth: 2.5),
                    )
                  : const Icon(Icons.download_rounded, color: Color(0xFF0F766E)),
              onTap: () => _abrirExamen(ex),
            ),
          );
        },
      ),
    );
  }
}