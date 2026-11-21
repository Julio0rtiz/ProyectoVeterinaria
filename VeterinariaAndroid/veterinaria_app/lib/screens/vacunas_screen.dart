import 'package:material_ui/material_ui.dart';
import 'package:intl/intl.dart';
import 'package:veterinaria_app/widgets/error_view.dart';
import '../models/mascota.dart';
import '../models/vacuna.dart';
import '../services/api_client.dart';
import '../services/auth_service.dart';
import '../services/veterinaria_api.dart';
import 'login_screen.dart';

class VacunasScreen extends StatefulWidget {
  final Mascota mascota;

  const VacunasScreen({super.key, required this.mascota});

  @override
  State<VacunasScreen> createState() => _VacunasScreenState();
}

class _VacunasScreenState extends State<VacunasScreen> {
  final _auth = AuthService();
  late final VeterinariaApi _api;
  List<Vacuna>? _vacunas;
  String? _error;
  bool _cargando = true;

  @override
  void initState() {
    super.initState();
    _api = VeterinariaApi(ApiClient(_auth));
    _cargarVacunas();
  }

  Future<void> _cargarVacunas() async {
    setState(() {
      _cargando = true;
      _error = null;
    });

    try {
      final datos = await _api.vacunas(widget.mascota.id);
      if (!mounted) return;
      setState(() {
        _vacunas = datos;
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
        _error = 'No se pudieron cargar las vacunas.';
        _cargando = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Vacunas • ${widget.mascota.nombre}'),
      ),
      body: _construirCuerpo(),
    );
  }

  Widget _construirCuerpo() {
    if (_cargando) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return ErrorView(mensaje: _error!, onReintentar: _cargarVacunas);
    }

    final lista = _vacunas ?? [];
    if (lista.isEmpty) {
      return const Center(
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Text(
            'No hay vacunas registradas para esta mascota.',
            textAlign: TextAlign.center,
            style: TextStyle(fontSize: 15, color: Color(0xFF475569)),
          ),
        ),
      );
    }

    final formato = DateFormat('dd/MM/yyyy');
    final hoy = DateTime.now();

    return RefreshIndicator(
      onRefresh: _cargarVacunas,
      child: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: lista.length,
        separatorBuilder: (_, _) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          final v = lista[index];
          final diasRestantes = v.fechaProximaDosis.difference(hoy).inDays;
          final vencePronto = !v.vencida && diasRestantes <= 30;

          Color bordeColor = const Color(0xFFDDE3E0);
          Color fondoBadge = const Color(0xFFE6F4F1);
          Color textoBadge = const Color(0xFF0F766E);
          String etiqueta = 'Al día';
          IconData icono = Icons.check_circle_outline;

          if (v.vencida) {
            bordeColor = const Color(0xFFFCA5A5);
            fondoBadge = const Color(0xFFFEF2F2);
            textoBadge = const Color(0xFFDC2626);
            etiqueta = 'Vencida';
            icono = Icons.warning_amber_rounded;
          } else if (vencePronto) {
            bordeColor = const Color(0xFFFCD34D);
            fondoBadge = const Color(0xFFFFFBEB);
            textoBadge = const Color(0xFFD97706);
            etiqueta = 'Vence pronto';
            icono = Icons.schedule;
          }

          return Card(
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(12),
              side: BorderSide(color: bordeColor, width: v.vencida ? 1.5 : 1),
            ),
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: Text(
                          v.nombre,
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: v.vencida ? const Color(0xFFDC2626) : const Color(0xFF1E293B),
                          ),
                        ),
                      ),
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                        decoration: BoxDecoration(
                          color: fondoBadge,
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(icono, size: 15, color: textoBadge),
                            const SizedBox(width: 4),
                            Text(
                              etiqueta,
                              style: TextStyle(
                                fontSize: 12,
                                fontWeight: FontWeight.bold,
                                color: textoBadge,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  Text(
                    'Aplicación: ${formato.format(v.fechaAplicacion)}',
                    style: const TextStyle(fontSize: 14, color: Color(0xFF334155)),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Próxima dosis: ${formato.format(v.fechaProximaDosis)}',
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: v.vencida ? FontWeight.bold : FontWeight.normal,
                      color: v.vencida ? const Color(0xFFDC2626) : const Color(0xFF334155),
                    ),
                  ),
                  const SizedBox(height: 4),
                  Text(
                    'Veterinario: ${v.veterinario}',
                    style: const TextStyle(fontSize: 13, color: Color(0xFF64748B)),
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