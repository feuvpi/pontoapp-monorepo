// lib/features/splash/pages/splash_page.dart
import 'package:flutter/material.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';

class SplashPage extends StatefulWidget {
  const SplashPage({super.key});

  @override
  State<SplashPage> createState() => _SplashPageState();
}

class _SplashPageState extends State<SplashPage>
    with SingleTickerProviderStateMixin {
  late AnimationController _fadeController;
  late Animation<double> _fadeAnimation;
  late Animation<Offset> _slideAnimation;

  @override
  void initState() {
    super.initState();

    _fadeController = AnimationController(
      duration: const Duration(milliseconds: 1500),
      vsync: this,
    );

    _fadeAnimation = CurvedAnimation(
      parent: _fadeController,
      curve: const Interval(0.0, 0.6, curve: Curves.easeOut),
    );

    _slideAnimation = Tween<Offset>(
      begin: const Offset(0, 0.3),
      end: Offset.zero,
    ).animate(CurvedAnimation(
      parent: _fadeController,
      curve: const Interval(0.2, 0.8, curve: Curves.easeOutCubic),
    ));

    _fadeController.forward();
  }

  @override
  void dispose() {
    _fadeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: Stack(
        children: [
          // Background layers
          _buildBackgroundLayers(),

          // Logo and content
          Center(
            child: SlideTransition(
              position: _slideAnimation,
              child: FadeTransition(
                opacity: _fadeAnimation,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    // Logo container with shadow
                    Container(
                      width: 110,
                      height: 110,
                      decoration: BoxDecoration(
                        gradient: const LinearGradient(
                          begin: Alignment.topLeft,
                          end: Alignment.bottomRight,
                          colors: [
                            AppColors.primary,
                            AppColors.primaryDark,
                          ],
                        ),
                        borderRadius: BorderRadius.circular(32),
                        boxShadow: [
                          BoxShadow(
                            color: AppColors.primary.withOpacity(0.4),
                            blurRadius: 30,
                            offset: const Offset(0, 15),
                          ),
                        ],
                      ),
                      child: const Icon(
                        Icons.access_time_rounded,
                        size: 55,
                        color: Colors.white,
                      ),
                    ),
                    const SizedBox(height: 28),
                    Text(
                      'PontoApp',
                      style: AppTextStyles.h1.copyWith(
                        fontWeight: FontWeight.w700,
                        letterSpacing: -0.5,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Registro de ponto simplificado',
                      style: AppTextStyles.bodyMedium.copyWith(
                        color: AppColors.textSecondary,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildBackgroundLayers() {
    return Stack(
      children: [
        // Layer 1 - Top wave (darker)
        Positioned(
          top: 0,
          left: 0,
          right: 0,
          child: ClipPath(
            clipper: _TopWaveClipper(),
            child: Container(
              height: MediaQuery.of(context).size.height * 0.45,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                  colors: [
                    AppColors.primary.withOpacity(0.08),
                    AppColors.primaryLight.withOpacity(0.05),
                  ],
                ),
              ),
            ),
          ),
        ),

        // Layer 2 - Second wave (lighter, offset)
        Positioned(
          top: 0,
          left: 0,
          right: 0,
          child: ClipPath(
            clipper: _TopWaveClipper(offset: 40),
            child: Container(
              height: MediaQuery.of(context).size.height * 0.42,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.topCenter,
                  end: Alignment.bottomCenter,
                  colors: [
                    AppColors.primary.withOpacity(0.05),
                    AppColors.primary.withOpacity(0.02),
                  ],
                ),
              ),
            ),
          ),
        ),

        // Layer 3 - Bottom wave
        Positioned(
          bottom: 0,
          left: 0,
          right: 0,
          child: ClipPath(
            clipper: _BottomWaveClipper(),
            child: Container(
              height: MediaQuery.of(context).size.height * 0.35,
              decoration: BoxDecoration(
                gradient: LinearGradient(
                  begin: Alignment.bottomLeft,
                  end: Alignment.topRight,
                  colors: [
                    AppColors.primary.withOpacity(0.06),
                    AppColors.primaryLight.withOpacity(0.03),
                  ],
                ),
              ),
            ),
          ),
        ),

        // Small decorative circle
        Positioned(
          top: MediaQuery.of(context).size.height * 0.15,
          right: 40,
          child: Container(
            width: 20,
            height: 20,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: AppColors.primary.withOpacity(0.15),
            ),
          ),
        ),

        // Another small circle
        Positioned(
          bottom: MediaQuery.of(context).size.height * 0.2,
          left: 50,
          child: Container(
            width: 12,
            height: 12,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: AppColors.primaryLight.withOpacity(0.2),
            ),
          ),
        ),
      ],
    );
  }
}

// Top wave clipper
class _TopWaveClipper extends CustomClipper<Path> {
  final double offset;

  _TopWaveClipper({this.offset = 0});

  @override
  Path getClip(Size size) {
    final path = Path();

    path.lineTo(0, size.height - 80 + offset);

    // First curve
    path.quadraticBezierTo(
      size.width * 0.25,
      size.height - 40 + offset,
      size.width * 0.5,
      size.height - 60 + offset,
    );

    // Second curve
    path.quadraticBezierTo(
      size.width * 0.75,
      size.height - 80 + offset,
      size.width,
      size.height - 20 + offset,
    );

    path.lineTo(size.width, 0);
    path.close();

    return path;
  }

  @override
  bool shouldReclip(covariant CustomClipper<Path> oldClipper) => false;
}

// Bottom wave clipper
class _BottomWaveClipper extends CustomClipper<Path> {
  @override
  Path getClip(Size size) {
    final path = Path();

    path.moveTo(0, size.height);
    path.lineTo(0, 80);

    // Wave curve
    path.quadraticBezierTo(
      size.width * 0.3,
      20,
      size.width * 0.5,
      50,
    );

    path.quadraticBezierTo(
      size.width * 0.7,
      80,
      size.width,
      30,
    );

    path.lineTo(size.width, size.height);
    path.close();

    return path;
  }

  @override
  bool shouldReclip(covariant CustomClipper<Path> oldClipper) => false;
}