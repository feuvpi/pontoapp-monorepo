import 'package:flutter/material.dart';
import 'package:pontoapp_mobile/core/theme/app_colors.dart';
import 'package:pontoapp_mobile/core/theme/app_text_styles.dart';
import 'package:pontoapp_mobile/core/theme/app_shadows.dart';

class TimeEntry {
  final String time;
  final String type; // 'in' or 'out'
  final DateTime dateTime;

  TimeEntry({
    required this.time,
    required this.type,
    required this.dateTime,
  });

  bool get isClockIn => type == 'in';
  bool get isClockOut => type == 'out';
}

class SummaryCard extends StatelessWidget {
  final List<TimeEntry> entries;
  final String totalWorked;
  final VoidCallback? onTapViewAll;

  const SummaryCard({
    super.key,
    required this.entries,
    required this.totalWorked,
    this.onTapViewAll,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: AppColors.surface,
        borderRadius: BorderRadius.circular(24),
        boxShadow: AppShadows.card,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Header
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Resumo de Hoje', style: AppTextStyles.h3),
              if (onTapViewAll != null)
                GestureDetector(
                  onTap: onTapViewAll,
                  child: Text(
                    'Ver tudo',
                    style: AppTextStyles.labelMedium.copyWith(
                      color: AppColors.primary,
                    ),
                  ),
                ),
            ],
          ),
          const SizedBox(height: 20),

          // Timeline
          if (entries.isEmpty)
            _buildEmptyState()
          else
            _buildTimeline(),

          const SizedBox(height: 20),

          // Total
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: AppColors.primary.withOpacity(0.08),
              borderRadius: BorderRadius.circular(16),
            ),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Total trabalhado',
                  style: AppTextStyles.bodyMedium.copyWith(
                    color: AppColors.textSecondary,
                  ),
                ),
                Text(
                  totalWorked,
                  style: AppTextStyles.h3.copyWith(
                    color: AppColors.primary,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildEmptyState() {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(vertical: 32),
      child: Column(
        children: [
          Icon(
            Icons.access_time_rounded,
            size: 48,
            color: AppColors.textHint.withOpacity(0.5),
          ),
          const SizedBox(height: 12),
          Text(
            'Nenhum registro hoje',
            style: AppTextStyles.bodyMedium.copyWith(
              color: AppColors.textHint,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTimeline() {
    // Group entries into pairs (in/out)
    final List<Widget> timelineItems = [];

    for (int i = 0; i < entries.length; i++) {
      final entry = entries[i];
      final nextEntry = i + 1 < entries.length ? entries[i + 1] : null;

      // Check if this is a clock-in with matching clock-out
      if (entry.isClockIn) {
        final hasMatchingOut = nextEntry != null && nextEntry.isClockOut;

        timelineItems.add(
          _TimelineRow(
            startTime: entry.time,
            endTime: hasMatchingOut ? nextEntry.time : null,
            isComplete: hasMatchingOut,
          ),
        );

        if (hasMatchingOut) {
          i++; // Skip the next entry since we've used it
        }
      }
    }

    if (timelineItems.isEmpty) {
      return _buildEmptyState();
    }

    return Column(
      children: timelineItems
          .map((item) => Padding(
                padding: const EdgeInsets.only(bottom: 12),
                child: item,
              ))
          .toList(),
    );
  }
}

class _TimelineRow extends StatelessWidget {
  final String startTime;
  final String? endTime;
  final bool isComplete;

  const _TimelineRow({
    required this.startTime,
    this.endTime,
    this.isComplete = false,
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      children: [
        // Start time
        SizedBox(
          width: 50,
          child: Text(
            startTime,
            style: AppTextStyles.labelLarge,
          ),
        ),

        // Start dot
        Container(
          width: 12,
          height: 12,
          decoration: BoxDecoration(
            color: AppColors.success,
            shape: BoxShape.circle,
            border: Border.all(
              color: AppColors.success.withOpacity(0.3),
              width: 3,
            ),
          ),
        ),

        // Line
        Expanded(
          child: Container(
            height: 3,
            margin: const EdgeInsets.symmetric(horizontal: 4),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: isComplete
                    ? [AppColors.success, AppColors.error]
                    : [AppColors.success, AppColors.success.withOpacity(0.3)],
              ),
              borderRadius: BorderRadius.circular(2),
            ),
          ),
        ),

        // End dot
        Container(
          width: 12,
          height: 12,
          decoration: BoxDecoration(
            color: isComplete ? AppColors.error : AppColors.textHint.withOpacity(0.3),
            shape: BoxShape.circle,
            border: Border.all(
              color: isComplete
                  ? AppColors.error.withOpacity(0.3)
                  : AppColors.textHint.withOpacity(0.2),
              width: 3,
            ),
          ),
        ),

        // End time
        SizedBox(
          width: 50,
          child: Text(
            endTime ?? '--:--',
            style: AppTextStyles.labelLarge.copyWith(
              color: isComplete ? AppColors.textPrimary : AppColors.textHint,
            ),
            textAlign: TextAlign.end,
          ),
        ),
      ],
    );
  }
}