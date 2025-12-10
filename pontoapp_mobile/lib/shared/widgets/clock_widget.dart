import 'dart:async';
import 'dart:math' as math;
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';
import 'package:pontoapp_mobile/core/theme/app_shadows.dart';

enum ClockStatus { idle, clockedIn, clockedOut, loading }

class ClockWidget extends StatefulWidget {
  final ClockStatus status;
  final VoidCallback? onTap;
  final bool enabled;

  const ClockWidget({
    super.key,
    this.status = ClockStatus.idle,
    this.onTap,
    this.enabled = true,
  });

  @override
  State<ClockWidget> createState() => _ClockWidgetState();
}

class _ClockWidgetState extends State<ClockWidget>
    with SingleTickerProviderStateMixin {
  late Timer _timer;
  DateTime _now = DateTime.now();
  bool _isPressed = false;
  late AnimationController _pulseController;
  late Animation<double> _pulseAnimation;

  @override
  void initState() {
    super.initState();
    _timer = Timer.periodic(const Duration(seconds: 1), (_) {
      setState(() => _now = DateTime.now());
    });

    _pulseController = AnimationController(
      duration: const Duration(milliseconds: 1500),
      vsync: this,
    );

    _pulseAnimation = Tween<double>(begin: 1.0, end: 1.05).animate(
      CurvedAnimation(parent: _pulseController, curve: Curves.easeInOut),
    );

    if (widget.status == ClockStatus.clockedIn) {
      _pulseController.repeat(reverse: true);
    }
  }

  @override
  void didUpdateWidget(ClockWidget oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (widget.status == ClockStatus.clockedIn) {
      _pulseController.repeat(reverse: true);
    } else {
      _pulseController.stop();
      _pulseController.reset();
    }
  }

  @override
  void dispose() {
    _timer.cancel();
    _pulseController.dispose();
    super.dispose();
  }

  String get _timeString {
    return '${_now.hour.toString().padLeft(2, '0')}:${_now.minute.toString().padLeft(2, '0')}';
  }

  String get _secondsString {
    return _now.second.toString().padLeft(2, '0');
  }

  String get _statusText {
    switch (widget.status) {
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

  Color get _statusColor {
    switch (widget.status) {
      case ClockStatus.idle:
        return AppColors.primary;
      case ClockStatus.clockedIn:
        return AppColors.success;
      case ClockStatus.clockedOut:
        return AppColors.textSecondary;
      case ClockStatus.loading:
        return AppColors.primary;
    }
  }

  Color get _ringColor {
    switch (widget.status) {
      case ClockStatus.idle:
        return AppColors.primary.withOpacity(0.2);
      case ClockStatus.clockedIn:
        return AppColors.success.withOpacity(0.3);
      case ClockStatus.clockedOut:
        return AppColors.textSecondary.withOpacity(0.2);
      case ClockStatus.loading:
        return AppColors.primary.withOpacity(0.2);
    }
  }

  void _handleTapDown(TapDownDetails details) {
    if (!widget.enabled || widget.status == ClockStatus.loading) return;
    setState(() => _isPressed = true);
    HapticFeedback.lightImpact();
  }

  void _handleTapUp(TapUpDetails details) {
    if (!widget.enabled || widget.status == ClockStatus.loading) return;
    setState(() => _isPressed = false);
    widget.onTap?.call();
    HapticFeedback.mediumImpact();
  }

  void _handleTapCancel() {
    setState(() => _isPressed = false);
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _pulseAnimation,
      builder: (context, child) {
        return Transform.scale(
          scale: widget.status == ClockStatus.clockedIn
              ? _pulseAnimation.value
              : 1.0,
          child: child,
        );
      },
      child: GestureDetector(
        onTapDown: _handleTapDown,
        onTapUp: _handleTapUp,
        onTapCancel: _handleTapCancel,
        child: AnimatedContainer(
          duration: const Duration(milliseconds: 150),
          width: 260,
          height: 260,
          decoration: BoxDecoration(
            color: AppColors.background,
            shape: BoxShape.circle,
            boxShadow: _isPressed
                ? AppShadows.neumorphismInset
                : AppShadows.neumorphismElevated,
          ),
          child: Stack(
            alignment: Alignment.center,
            children: [
              // Progress ring
              CustomPaint(
                size: const Size(240, 240),
                painter: _ClockRingPainter(
                  progress: widget.status == ClockStatus.clockedIn
                      ? _now.second / 60
                      : 0,
                  color: _ringColor,
                  strokeWidth: 8,
                ),
              ),

              // Inner circle
              AnimatedContainer(
                duration: const Duration(milliseconds: 150),
                width: 200,
                height: 200,
                decoration: BoxDecoration(
                  color: AppColors.surface,
                  shape: BoxShape.circle,
                  boxShadow: _isPressed ? [] : AppShadows.soft,
                ),
                child: widget.status == ClockStatus.loading
                    ? const Center(
                        child: CircularProgressIndicator(
                          strokeWidth: 3,
                          color: AppColors.primary,
                        ),
                      )
                    : Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          // Time
                          Row(
                            mainAxisAlignment: MainAxisAlignment.center,
                            crossAxisAlignment: CrossAxisAlignment.end,
                            children: [
                              Text(_timeString, style: AppTextStyles.clockLarge),
                              Padding(
                                padding: const EdgeInsets.only(bottom: 8),
                                child: Text(
                                  _secondsString,
                                  style: AppTextStyles.bodyMedium.copyWith(
                                    color: AppColors.textSecondary,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          const SizedBox(height: 8),
                          // Status indicator
                          Container(
                            padding: const EdgeInsets.symmetric(
                              horizontal: 12,
                              vertical: 4,
                            ),
                            decoration: BoxDecoration(
                              color: _statusColor.withOpacity(0.1),
                              borderRadius: BorderRadius.circular(12),
                            ),
                            child: Row(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Container(
                                  width: 8,
                                  height: 8,
                                  decoration: BoxDecoration(
                                    color: _statusColor,
                                    shape: BoxShape.circle,
                                  ),
                                ),
                                const SizedBox(width: 6),
                                Text(
                                  _statusLabel,
                                  style: AppTextStyles.labelMedium.copyWith(
                                    color: _statusColor,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  String get _statusLabel {
    switch (widget.status) {
      case ClockStatus.idle:
        return 'Entrada';
      case ClockStatus.clockedIn:
        return 'Trabalhando';
      case ClockStatus.clockedOut:
        return 'Encerrado';
      case ClockStatus.loading:
        return 'Aguarde';
    }
  }
}

class _ClockRingPainter extends CustomPainter {
  final double progress;
  final Color color;
  final double strokeWidth;

  _ClockRingPainter({
    required this.progress,
    required this.color,
    required this.strokeWidth,
  });

  @override
  void paint(Canvas canvas, Size size) {
    final center = Offset(size.width / 2, size.height / 2);
    final radius = (size.width - strokeWidth) / 2;

    // Background ring
    final bgPaint = Paint()
      ..color = color.withOpacity(0.3)
      ..style = PaintingStyle.stroke
      ..strokeWidth = strokeWidth
      ..strokeCap = StrokeCap.round;

    canvas.drawCircle(center, radius, bgPaint);

    // Progress arc
    if (progress > 0) {
      final progressPaint = Paint()
        ..color = color
        ..style = PaintingStyle.stroke
        ..strokeWidth = strokeWidth
        ..strokeCap = StrokeCap.round;

      canvas.drawArc(
        Rect.fromCircle(center: center, radius: radius),
        -math.pi / 2,
        2 * math.pi * progress,
        false,
        progressPaint,
      );
    }
  }

  @override
  bool shouldRepaint(_ClockRingPainter oldDelegate) {
    return oldDelegate.progress != progress || oldDelegate.color != color;
  }
}