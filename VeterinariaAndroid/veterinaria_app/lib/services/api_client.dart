import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'package:http/http.dart' as http;
import '../config.dart';
import 'auth_service.dart';

class ApiException implements Exception {
  final String mensaje;
  final bool sesionExpirada;

  ApiException(this.mensaje, {this.sesionExpirada = false});
}

class ApiClient {
  final AuthService _auth;

  ApiClient(this._auth);

  Future<http.Response> _get(String ruta) async {
    final token = await _auth.leerToken();
    try {
      final r = await http.get(
        Uri.parse('$baseUrl$ruta'),
        headers: {'Authorization': 'Bearer $token'},
      ).timeout(const Duration(seconds: 15));

      if (r.statusCode == 401) {
        await _auth.cerrarSesion();
        throw ApiException('Su sesión expiró. Inicie sesión de nuevo.', sesionExpirada: true);
      }
      if (r.statusCode >= 400) {
        throw ApiException(_mensaje(r) ?? 'Ocurrió un error (${r.statusCode}).');
      }
      return r;
    } on SocketException {
      throw ApiException('No hay conexión a internet. Revise su red e intente de nuevo.');
    } on TimeoutException {
      throw ApiException('El servidor tardó demasiado en responder. Intente de nuevo.');
    } on http.ClientException {
      throw ApiException('No se pudo conectar con el servidor.');
    }
  }

  Future<dynamic> getJson(String ruta) async => jsonDecode((await _get(ruta)).body);

  Future<List<int>> getBytes(String ruta) async => (await _get(ruta)).bodyBytes;

  String? _mensaje(http.Response r) {
    try {
      return jsonDecode(r.body)['mensaje'] as String?;
    } catch (_) {
      return null;
    }
  }
}