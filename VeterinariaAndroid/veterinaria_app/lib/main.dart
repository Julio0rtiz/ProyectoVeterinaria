import 'package:flutter/material.dart';
import 'screens/login_screen.dart';
import 'screens/mascotas_screen.dart';
import 'services/auth_service.dart';
import 'widgets/logo_clinica.dart';

void main() {
  runApp(const ClinicaVeterinariaApp());
}

class ClinicaVeterinariaApp extends StatelessWidget {
  const ClinicaVeterinariaApp({super.key});

  @override
  Widget build(BuildContext context) {
    const primario = Color(0xFF0F766E);
    const fondoHoja = Color(0xFFF4F6F5);

    return MaterialApp(
      title: 'Clínica Veterinaria',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        useMaterial3: true,
        colorScheme: ColorScheme.fromSeed(
          seedColor: primario,
          primary: primario,
          surface: Colors.white,
        ),
        scaffoldBackgroundColor: fondoHoja,
        appBarTheme: const AppBarTheme(
          backgroundColor: primario,
          foregroundColor: Colors.white,
          elevation: 0,
          centerTitle: false,
        ),
        cardTheme: CardThemeData(
          color: Colors.white,
          elevation: 0,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(12),
            side: const BorderSide(color: Color(0xFFDDE3E0)),
          ),
        ),
        inputDecorationTheme: InputDecorationTheme(
          filled: true,
          fillColor: Colors.white,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Color(0xFFCBD5E1)),
          ),
          enabledBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: Color(0xFFCBD5E1)),
          ),
          focusedBorder: OutlineInputBorder(
            borderRadius: BorderRadius.circular(10),
            borderSide: const BorderSide(color: primario, width: 2),
          ),
        ),
        filledButtonTheme: FilledButtonThemeData(
          style: FilledButton.styleFrom(
            backgroundColor: primario,
            foregroundColor: Colors.white,
            minimumSize: const Size.fromHeight(48),
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(10),
            ),
          ),
        ),
      ),
      home: const VerificadorSesion(),
    );
  }
}

class VerificadorSesion extends StatefulWidget {
  const VerificadorSesion({super.key});

  @override
  State<VerificadorSesion> createState() => _VerificadorSesionState();
}

class _VerificadorSesionState extends State<VerificadorSesion> {
  final _auth = AuthService();

  @override
  void initState() {
    super.initState();
    _verificar();
  }

  Future<void> _verificar() async {
    final token = await _auth.leerToken();
    if (!mounted) return;
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(
        builder: (_) => token != null && token.isNotEmpty
            ? const MascotasScreen()
            : const LoginScreen(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return const Scaffold(
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            LogoClinica(tamano: 84),
            SizedBox(height: 16),
            Text(
              'Clínica Veterinaria',
              style: TextStyle(
                fontSize: 22,
                fontWeight: FontWeight.bold,
                color: Color(0xFF1E293B),
              ),
            ),
            SizedBox(height: 24),
            CircularProgressIndicator(),
          ],
        ),
      ),
    );
  }
}