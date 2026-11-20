import 'package:flutter_test/flutter_test.dart';
import 'package:veterinaria_app/main.dart';

void main() {
  testWidgets('Inicia la aplicación correctamente', (WidgetTester tester) async {
    await tester.pumpWidget(const ClinicaVeterinariaApp());
  });
}