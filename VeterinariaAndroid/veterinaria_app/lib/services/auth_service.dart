import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;
import '../config.dart';
import 'api_client.dart';

class AuthService {
  final _storage = const FlutterSecureStorage();
  static const _tokenKey = 'jwt_token';

  Future<void> login(String correo, String contrasena) async {
    final r = await http.post(
      Uri.parse('$baseUrl/api/auth/login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'correo': correo, 'contrasena': contrasena}),
    ).timeout(const Duration(seconds: 15));

    if (r.statusCode == 200) {
      final data = jsonDecode(r.body);
      if (data['perfil'] != 'Dueno') {
        throw ApiException('Esta aplicación es exclusiva para dueños.');
      }
      await _storage.write(key: _tokenKey, value: data['token']);
      return;
    }

    final mensaje = _extraerMensaje(r) ?? 'Error al iniciar sesión (${r.statusCode}).';
    throw ApiException(mensaje);
  }

  Future<void> registro(String cedula, String nombre, String correo, String contrasena) async {
    final r = await http.post(
      Uri.parse('$baseUrl/api/auth/registro'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({
        'cedula': cedula,
        'nombre': nombre,
        'correo': correo,
        'contrasena': contrasena,
      }),
    ).timeout(const Duration(seconds: 15));

    if (r.statusCode == 201) {
      final data = jsonDecode(r.body);
      await _storage.write(key: _tokenKey, value: data['token']);
      return;
    }

    final mensaje = _extraerMensaje(r) ?? 'Error al registrar usuario (${r.statusCode}).';
    throw ApiException(mensaje);
  }

  Future<String?> leerToken() => _storage.read(key: _tokenKey);

  Future<void> cerrarSesion() => _storage.delete(key: _tokenKey);

  String? _extraerMensaje(http.Response r) {
    try {
      return jsonDecode(r.body)['mensaje'] as String?;
    } catch (_) {
      return null;
    }
  }
}