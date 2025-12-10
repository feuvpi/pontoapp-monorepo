// lib/features/splash/pages/splash_page.dart
import 'package:flutter/material.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';
import 'package:pontoapp_mobile/shared/widgets/clock_widget.dart';

class SplashPage extends StatefulWidget {
  const SplashPage({super.key});

  @override
  State<SplashPage> createState() => _SplashPageState();
}

class _SplashPageState extends State<SplashPage> {
  ClockStatus _status = ClockStatus.idle;

  void _handleClockTap() {
    setState(() {
      if (_status == ClockStatus.idle) {
        _status = ClockStatus.loading;
        Future.delayed(const Duration(seconds: 2), () {
          setState(() => _status = ClockStatus.clockedIn);
        });
      } else if (_status == ClockStatus.clockedIn) {
        _status = ClockStatus.loading;
        Future.delayed(const Duration(seconds: 2), () {
          setState(() => _status = ClockStatus.clockedOut);
        });
      } else if (_status == ClockStatus.clockedOut) {
        _status = ClockStatus.idle;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text('Bom dia, João!', style: AppTextStyles.h2),
              const SizedBox(height: 8),
              Text(
                'Terça, 3 de Dezembro',
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
              const SizedBox(height: 48),
              ClockWidget(
                status: _status,
                onTap: _handleClockTap,
              ),
              const SizedBox(height: 24),
              Text(
                _getStatusText(),
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _getStatusText() {
    switch (_status) {
      case ClockStatus.idle:
        return 'Toque para registrar entrada';
      case ClockStatus.clockedIn:
        return 'Toque para registrar saída';
      case ClockStatus.clockedOut:
        return 'Expediente encerrado';
      case ClockStatus.loading:
        return 'Registrando...';
    }
  }
}