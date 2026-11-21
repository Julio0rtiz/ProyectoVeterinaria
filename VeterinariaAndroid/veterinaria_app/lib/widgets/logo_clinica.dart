import 'package:material_ui/material_ui.dart';

class LogoClinica extends StatelessWidget {
  final double tamano;

  const LogoClinica({super.key, this.tamano = 76});

  @override
  Widget build(BuildContext context) {
    return Container(
      width: tamano,
      height: tamano,
      decoration: BoxDecoration(
        color: const Color(0xFF0F766E),
        borderRadius: BorderRadius.circular(tamano * 0.24),
        boxShadow: [
          BoxShadow(
            color: const Color(0xFF0F766E).withValues(alpha: 0.22),
            blurRadius: 14,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Icon(
        Icons.monitor_heart_outlined,
        size: tamano * 0.56,
        color: const Color(0xFFE5A947),
      ),
    );
  }
}