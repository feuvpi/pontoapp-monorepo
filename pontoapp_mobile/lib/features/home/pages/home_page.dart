// lib/features/home/pages/home_page.dart
import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';
import 'package:pontoapp_mobile/shared/widgets/clock_widget.dart';
import 'package:pontoapp_mobile/shared/widgets/summary_card.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  ClockStatus _clockStatus = ClockStatus.idle;
  final List<TimeEntry> _entries = [];
  String _totalWorked = '0h 00min';

  String get _greeting {
    final hour = DateTime.now().hour;
    if (hour < 12) return 'Bom dia';
    if (hour < 18) return 'Boa tarde';
    return 'Boa noite';
  }

  String get _formattedDate {
    final now = DateTime.now();
    final formatter = DateFormat("EEEE, d 'de' MMMM", 'pt_BR');
    final formatted = formatter.format(now);
    // Capitalize first letter
    return formatted[0].toUpperCase() + formatted.substring(1);
  }

  void _handleClockTap() {
    if (_clockStatus == ClockStatus.loading) return;

    setState(() => _clockStatus = ClockStatus.loading);

    // Simulate API call
    Future.delayed(const Duration(seconds: 2), () {
      setState(() {
        final now = DateTime.now();
        final timeStr = DateFormat('HH:mm').format(now);

        if (_clockStatus == ClockStatus.loading) {
          // Check if we need to clock in or out
          final needsClockIn = _entries.isEmpty ||
              _entries.last.isClockOut;

          if (needsClockIn) {
            _entries.add(TimeEntry(
              time: timeStr,
              type: 'in',
              dateTime: now,
            ));
            _clockStatus = ClockStatus.clockedIn;
          } else {
            _entries.add(TimeEntry(
              time: timeStr,
              type: 'out',
              dateTime: now,
            ));
            _clockStatus = ClockStatus.clockedOut;
          }

          _calculateTotalWorked();
        }
      });
    });
  }

  void _calculateTotalWorked() {
    int totalMinutes = 0;

    for (int i = 0; i < _entries.length; i += 2) {
      final clockIn = _entries[i];
      final clockOut = i + 1 < _entries.length ? _entries[i + 1] : null;

      if (clockOut != null) {
        final diff = clockOut.dateTime.difference(clockIn.dateTime);
        totalMinutes += diff.inMinutes;
      } else {
        // Still working - calculate from clock in to now
        final diff = DateTime.now().difference(clockIn.dateTime);
        totalMinutes += diff.inMinutes;
      }
    }

    final hours = totalMinutes ~/ 60;
    final minutes = totalMinutes % 60;
    setState(() {
      _totalWorked = '${hours}h ${minutes.toString().padLeft(2, '0')}min';
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 24),
          child: Column(
            children: [
              const SizedBox(height: 16),

              // Header
              _buildHeader(),

              const SizedBox(height: 32),

              // Greeting
              _buildGreeting(),

              const SizedBox(height: 40),

              // Clock
              ClockWidget(
                status: _clockStatus,
                onTap: _handleClockTap,
              ),

              const SizedBox(height: 16),

              // Status text
              Text(
                _getStatusText(),
                style: AppTextStyles.bodyMedium.copyWith(
                  color: AppColors.textSecondary,
                ),
              ),

              const SizedBox(height: 40),

              // Summary
              SummaryCard(
                entries: _entries,
                totalWorked: _totalWorked,
                onTapViewAll: () {
                  // TODO: Navigate to history
                },
              ),

              const SizedBox(height: 100), // Space for bottom nav
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Row(
          children: [
            Container(
              width: 8,
              height: 8,
              decoration: const BoxDecoration(
                color: AppColors.success,
                shape: BoxShape.circle,
              ),
            ),
            const SizedBox(width: 8),
            Text(
              _formattedDate,
              style: AppTextStyles.bodySmall,
            ),
          ],
        ),
        IconButton(
          onPressed: () {
            // TODO: Navigate to settings
          },
          icon: const Icon(
            Icons.settings_outlined,
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }

  Widget _buildGreeting() {
    return Column(
      children: [
        Text(
          '$_greeting, João! 👋',
          style: AppTextStyles.h2,
        ),
      ],
    );
  }

  String _getStatusText() {
    switch (_clockStatus) {
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