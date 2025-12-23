// lib/features/home/pages/home_page.dart
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:intl/intl.dart';
import 'package:pontoapp_mobile/core/di/injection.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';
import 'package:pontoapp_mobile/shared/widgets/clock_widget.dart';
import 'package:pontoapp_mobile/shared/widgets/summary_card.dart';
import 'package:pontoapp_mobile/features/home/bloc/home_bloc.dart';
import 'package:pontoapp_mobile/features/home/bloc/home_event.dart';
import 'package:pontoapp_mobile/features/home/bloc/home_state.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  late final HomeBloc _homeBloc;

  @override
  void initState() {
    super.initState();
    _homeBloc = getIt<HomeBloc>();
    _homeBloc.add(HomeLoadRequested());
  }

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
    return formatted[0].toUpperCase() + formatted.substring(1);
  }

  void _handleClockTap(HomeState state) {
    if (state.isClocking) return;

    if (state.nextAction == ClockAction.clockIn) {
      _homeBloc.add(HomeClockInRequested());
    } else {
      _homeBloc.add(HomeClockOutRequested());
    }
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider.value(
      value: _homeBloc,
      child: Scaffold(
        backgroundColor: AppColors.background,
        body: SafeArea(
          child: BlocConsumer<HomeBloc, HomeState>(
            listener: (context, state) {
              if (state.error != null) {
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(
                    content: Text(state.error!),
                    backgroundColor: AppColors.error,
                    behavior: SnackBarBehavior.floating,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12),
                    ),
                  ),
                );
              }
            },
            builder: (context, state) {
              if (state.isLoading) {
                return const Center(
                  child: CircularProgressIndicator(color: AppColors.primary),
                );
              }

              return RefreshIndicator(
                onRefresh: () async {
                  _homeBloc.add(HomeRefreshRequested());
                },
                child: SingleChildScrollView(
                  physics: const AlwaysScrollableScrollPhysics(),
                  padding: const EdgeInsets.symmetric(horizontal: 24),
                  child: Column(
                    children: [
                      const SizedBox(height: 16),
                      _buildHeader(),
                      const SizedBox(height: 32),
                      _buildGreeting(state),
                      const SizedBox(height: 40),
                      _buildClock(state),
                      const SizedBox(height: 16),
                      _buildStatusText(state),
                      const SizedBox(height: 40),
                      _buildSummary(state),
                      const SizedBox(height: 100),
                    ],
                  ),
                ),
              );
            },
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
            Text(_formattedDate, style: AppTextStyles.bodySmall),
          ],
        ),
        IconButton(
          onPressed: () {
            // TODO: Navigate to settings/profile
          },
          icon: const Icon(
            Icons.settings_outlined,
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }

  Widget _buildGreeting(HomeState state) {
    return Column(
      children: [
        Text(
          '$_greeting, ${state.userName.split(' ').first}! 👋',
          style: AppTextStyles.h2,
        ),
      ],
    );
  }

  Widget _buildClock(HomeState state) {
    ClockStatus clockStatus;

    if (state.isClocking) {
      clockStatus = ClockStatus.loading;
    } else if (state.nextAction == ClockAction.clockOut) {
      clockStatus = ClockStatus.clockedIn;
    } else if (state.summary != null && state.summary!.records.isNotEmpty) {
      // Tem registros mas próxima ação é clockIn = já fez clockOut
      final hasCompleteDay = state.summary!.records.length >= 2 &&
          state.summary!.records.length % 2 == 0;
      clockStatus = hasCompleteDay ? ClockStatus.clockedOut : ClockStatus.idle;
    } else {
      clockStatus = ClockStatus.idle;
    }

    return ClockWidget(
      status: clockStatus,
      onTap: () => _handleClockTap(state),
    );
  }

  Widget _buildStatusText(HomeState state) {
    String text;

    if (state.isClocking) {
      text = 'Registrando...';
    } else if (state.nextAction == ClockAction.clockIn) {
      text = 'Toque para registrar entrada';
    } else {
      text = 'Toque para registrar saída';
    }

    return Text(
      text,
      style: AppTextStyles.bodyMedium.copyWith(
        color: AppColors.textSecondary,
      ),
    );
  }

  Widget _buildSummary(HomeState state) {
    final entries = state.summary?.records.map((r) {
      return TimeEntry(
        time: DateFormat('HH:mm').format(r.recordedAt),
        type: r.isClockIn ? 'in' : 'out',
        dateTime: r.recordedAt,
      );
    }).toList() ?? [];

    return SummaryCard(
      entries: entries,
      totalWorked: state.summary?.totalWorkedFormatted ?? '0h 00min',
      onTapViewAll: () {
        // TODO: Navigate to history
      },
    );
  }
}