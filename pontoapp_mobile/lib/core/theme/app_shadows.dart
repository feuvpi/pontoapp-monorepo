// lib/core/theme/app_shadows.dart
import 'package:flutter/material.dart';
import 'app_colors.dart';

abstract class AppShadows {
  // Neumorphism - Elevated (default state)
  static List<BoxShadow> get neumorphismElevated => [
        const BoxShadow(
          color: AppColors.shadowDark,
          offset: Offset(6, 6),
          blurRadius: 12,
        ),
        const BoxShadow(
          color: AppColors.shadowLight,
          offset: Offset(-6, -6),
          blurRadius: 12,
        ),
      ];

  // Neumorphism - Pressed (active state)
  static List<BoxShadow> get neumorphismPressed => [
        const BoxShadow(
          color: AppColors.shadowDark,
          offset: Offset(2, 2),
          blurRadius: 4,
        ),
        const BoxShadow(
          color: AppColors.shadowLight,
          offset: Offset(-2, -2),
          blurRadius: 4,
        ),
      ];

  // Neumorphism - Inset (pressed inward)
  static List<BoxShadow> get neumorphismInset => [
        BoxShadow(
          color: AppColors.shadowDark.withOpacity(0.5),
          offset: const Offset(3, 3),
          blurRadius: 6,
          spreadRadius: -2,
        ),
        const BoxShadow(
          color: AppColors.shadowLight,
          offset: Offset(-3, -3),
          blurRadius: 6,
          spreadRadius: -2,
        ),
      ];

  // Card shadow
  static List<BoxShadow> get card => [
        BoxShadow(
          color: AppColors.shadowDark.withOpacity(0.1),
          offset: const Offset(0, 4),
          blurRadius: 12,
        ),
      ];

  // Soft shadow
  static List<BoxShadow> get soft => [
        BoxShadow(
          color: AppColors.shadowDark.withOpacity(0.08),
          offset: const Offset(0, 2),
          blurRadius: 8,
        ),
      ];
}