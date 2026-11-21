import 'package:material_ui/material_ui.dart';
import '../models/mascota.dart';
import '../services/api_client.dart';
import '../services/auth_service.dart';
import '../services/veterinaria_api.dart';
import '../widgets/error_view.dart';
import 'login_screen.dart';
import 'menu_mascota_screen.dart';

class MascotasScreen extends StatefulWidget {
  const MascotasScreen({super.key});

  @override
  State<MascotasScreen> createState() => _MascotasScreenState();
}

class _MascotasScreenState extends State<MascotasScreen> {
  final _auth = AuthService();
  late final VeterinariaApi _api;
  List<Mascota>? _mascotas;
  String? _error;
  bool _cargando = true;

  @override
  void initState() {
    super.initState();
    _api = VeterinariaApi(ApiClient(_auth));
    _cargarMascotas();
  }

  Future<void> _cargarMascotas() async {
    setState(() {
      _cargando = true;
      _error = null;
    });

    try {
      final datos = await _api.misMascotas();
      if (!mounted) return;
      setState(() {
        _mascotas = datos;
        _cargando = false;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      if (e.sesionExpirada) {
        _irALoginConMensaje(e.mensaje);
        return;
      }
      setState(() {
        _error = e.mensaje;
        _cargando = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        _error = 'Ocurrió un error al cargar sus mascotas.';
        _cargando = false;
      });
    }
  }

  void _irALoginConMensaje(String mensaje) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(mensaje)),
    );
    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (_) => const LoginScreen()),
      (route) => false,
    );
  }

  Future<void> _cerrarSesion() async {
    await _auth.cerrarSesion();
    if (!mounted) return;
    Navigator.pushAndRemoveUntil(
      context,
      MaterialPageRoute(builder: (_) => const LoginScreen()),
      (route) => false,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Row(
          children: [
            Icon(
              Icons.monitor_heart_outlined,
              size: 22,
              color: Color(0xFFE5A947),
            ),
            SizedBox(width: 8),
            Text('Mis mascotas'),
          ],
        ),
        actions: [
          IconButton(
            tooltip: 'Recargar',
            icon: const Icon(Icons.refresh),
            onPressed: _cargando ? null : _cargarMascotas,
          ),
          IconButton(
            tooltip: 'Cerrar sesión',
            icon: const Icon(Icons.logout),
            onPressed: _cerrarSesion,
          ),
        ],
      ),
      body: _construirCuerpo(),
    );
  }

  Widget _construirCuerpo() {
    if (_cargando) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error != null) {
      return ErrorView(
        mensaje: _error!,
        onReintentar: _cargarMascotas,
      );
    }

    final lista = _mascotas ?? [];

    return RefreshIndicator(
      onRefresh: _cargarMascotas,
      child: lista.isEmpty
          ? ListView(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(24),
              children: [
                const SizedBox(height: 60),
                Card(
                  child: Padding(
                    padding: const EdgeInsets.all(28),
                    child: Column(
                      children: [
                        Icon(
                          Icons.pets,
                          size: 52,
                          color: const Color(0xFFE5A947),
                        ),
                        const SizedBox(height: 16),
                        const Text(
                          'Aún no tiene mascotas registradas en la clínica',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontSize: 16,
                            color: Color(0xFF475569),
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ],
            )
          : ListView.separated(
              physics: const AlwaysScrollableScrollPhysics(),
              padding: const EdgeInsets.all(16),
              itemCount: lista.length,
              separatorBuilder: (_, _) => const SizedBox(height: 12),
              itemBuilder: (context, index) {
                final mascota = lista[index];
                return Card(
                  child: InkWell(
                    borderRadius: BorderRadius.circular(12),
                    onTap: () {
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (_) => MenuMascotaScreen(mascota: mascota),
                        ),
                      );
                    },
                    child: Padding(
                      padding: const EdgeInsets.all(12),
                      child: Row(
                        children: [
                          _FotoMascota(url: mascota.fotoUrl, tamano: 64),
                          const SizedBox(width: 14),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  mascota.nombre,
                                  style: const TextStyle(
                                    fontSize: 17,
                                    fontWeight: FontWeight.bold,
                                    color: Color(0xFF1E293B),
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  '${mascota.especie} • ${mascota.raza}',
                                  style: const TextStyle(
                                    fontSize: 14,
                                    color: Color(0xFF475569),
                                  ),
                                ),
                              ],
                            ),
                          ),
                          const Icon(
                            Icons.chevron_right,
                            color: Color(0xFF64748B),
                          ),
                        ],
                      ),
                    ),
                  ),
                );
              },
            ),
    );
  }
}

class _FotoMascota extends StatelessWidget {
  final String? url;
  final double tamano;

  const _FotoMascota({required this.url, required this.tamano});

  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: BorderRadius.circular(10),
      child: Container(
        width: tamano,
        height: tamano,
        color: const Color(0xFFE6F4F1),
        child: url != null && url!.isNotEmpty
            ? Image.network(
                url!,
                fit: BoxFit.cover,
                errorBuilder: (_, _, _) => const Icon(
                  Icons.pets,
                  color: Color(0xFFE5A947),
                  size: 32,
                ),
              )
            : const Icon(
                Icons.pets,
                color: Color(0xFFE5A947),
                size: 32,
              ),
      ),
    );
  }
}