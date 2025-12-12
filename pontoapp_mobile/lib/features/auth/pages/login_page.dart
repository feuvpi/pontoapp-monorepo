import 'package:flutter/material.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';
import 'package:pontoapp_mobile/core/theme/app_shadows.dart';
import 'package:pontoapp_mobile/features/auth/widgets/login_form.dart';

class LoginPage extends StatelessWidget {
  const LoginPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24),
          child: Column(
            children: [
              const SizedBox(height: 60),

              // Logo
              _buildLogo(),

              const SizedBox(height: 48),

              // Welcome text
              _buildWelcomeText(),

              const SizedBox(height: 40),

              // Login form
              const LoginForm(),

              const SizedBox(height: 24),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildLogo() {
    return Container(
      width: 100,
      height: 100,
      decoration: BoxDecoration(
        color: AppColors.background,
        borderRadius: BorderRadius.circular(28),
        boxShadow: AppShadows.neumorphismElevated,
      ),
      child: const Icon(
        Icons.access_time_rounded,
        size: 50,
        color: AppColors.primary,
      ),
    );
  }

  Widget _buildWelcomeText() {
    return Column(
      children: [
        Text(
          'Bem-vindo ao PontoApp',
          style: AppTextStyles.h2,
          textAlign: TextAlign.center,
        ),
        const SizedBox(height: 8),
        Text(
          'Faça login para registrar seu ponto',
          style: AppTextStyles.bodyMedium.copyWith(
            color: AppColors.textSecondary,
          ),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }
}