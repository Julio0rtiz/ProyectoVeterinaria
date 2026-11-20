import '../models/examen.dart';
import '../models/historial.dart';
import '../models/mascota.dart';
import '../models/tratamiento.dart';
import '../models/vacuna.dart';
import 'api_client.dart';

class VeterinariaApi {
  final ApiClient _client;

  VeterinariaApi(this._client);

  Future<List<Mascota>> misMascotas() async {
    final lista = await _client.getJson('/api/mascotas') as List<dynamic>;
    return lista.map((e) => Mascota.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<List<Vacuna>> vacunas(int mascotaId) async {
    final lista = await _client.getJson('/api/mascotas/$mascotaId/vacunas') as List<dynamic>;
    return lista.map((e) => Vacuna.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<List<Examen>> examenes(int mascotaId) async {
    final lista = await _client.getJson('/api/mascotas/$mascotaId/examenes') as List<dynamic>;
    return lista.map((e) => Examen.fromJson(e as Map<String, dynamic>)).toList();
  }

  Future<List<int>> descargarExamen(int examenId) async {
    return await _client.getBytes('/api/examenes/$examenId/archivo');
  }

  Future<TratamientosResponse> tratamientos(int mascotaId) async {
    final data = await _client.getJson('/api/mascotas/$mascotaId/tratamientos') as Map<String, dynamic>;
    return TratamientosResponse.fromJson(data);
  }

  Future<List<NotaHistorial>> historial(int mascotaId) async {
    final lista = await _client.getJson('/api/mascotas/$mascotaId/historial') as List<dynamic>;
    return lista.map((e) => NotaHistorial.fromJson(e as Map<String, dynamic>)).toList();
  }
}