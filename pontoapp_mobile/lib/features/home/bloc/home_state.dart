import 'package:equatable/equatable.dart';
import 'package:pontoapp_mobile/models/time_record.dart';

enum ClockAction { none, clockIn, clockOut }

class HomeState extends Equatable {
  final bool isLoading;
  final bool isClocking;
  final String? error;
  final String userName;
  final DailySummary? summary;
  final ClockAction nextAction;

  const HomeState({
    this.isLoading = false,
    this.isClocking = false,
    this.error,
    this.userName = '',
    this.summary,
    this.nextAction = ClockAction.clockIn,
  });

  HomeState copyWith({
    bool? isLoading,
    bool? isClocking,
    String? error,
    String? userName,
    DailySummary? summary,
    ClockAction? nextAction,
  }) {
    return HomeState(
      isLoading: isLoading ?? this.isLoading,
      isClocking: isClocking ?? this.isClocking,
      error: error,
      userName: userName ?? this.userName,
      summary: summary ?? this.summary,
      nextAction: nextAction ?? this.nextAction,
    );
  }

  @override
  List<Object?> get props => [isLoading, isClocking, error, userName, summary, nextAction];
}