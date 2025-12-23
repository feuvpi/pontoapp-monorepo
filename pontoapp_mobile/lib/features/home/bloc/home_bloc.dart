import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:pontoapp_mobile/models/time_record.dart';
import 'package:pontoapp_mobile/services/auth_service.dart';
import 'package:pontoapp_mobile/services/time_record_service.dart';
import 'home_event.dart';
import 'home_state.dart';

class HomeBloc extends Bloc<HomeEvent, HomeState> {
  final AuthService _authService;
  final TimeRecordService _timeRecordService;

  HomeBloc(this._authService, this._timeRecordService) : super(const HomeState()) {
    on<HomeLoadRequested>(_onLoadRequested);
    on<HomeClockInRequested>(_onClockInRequested);
    on<HomeClockOutRequested>(_onClockOutRequested);
    on<HomeRefreshRequested>(_onRefreshRequested);
  }

  Future<void> _onLoadRequested(
    HomeLoadRequested event,
    Emitter<HomeState> emit,
  ) async {
    emit(state.copyWith(isLoading: true, error: null));

    final userName = await _authService.getUserName() ?? 'Usuário';

    final result = await _timeRecordService.getDailySummary();

    result.fold(
      (failure) => emit(state.copyWith(
        isLoading: false,
        userName: userName,
        error: failure.message,
        nextAction: ClockAction.clockIn,
      )),
      (summary) => emit(state.copyWith(
        isLoading: false,
        userName: userName,
        summary: summary,
        nextAction: _determineNextAction(summary),
      )),
    );
  }

  Future<void> _onClockInRequested(
    HomeClockInRequested event,
    Emitter<HomeState> emit,
  ) async {
    emit(state.copyWith(isClocking: true, error: null));

    final result = await _timeRecordService.clockIn();

    result.fold(
      (failure) => emit(state.copyWith(
        isClocking: false,
        error: failure.message,
      )),
      (record) {
        // Recarregar summary após clock-in
        add(HomeRefreshRequested());
      },
    );
  }

  Future<void> _onClockOutRequested(
    HomeClockOutRequested event,
    Emitter<HomeState> emit,
  ) async {
    emit(state.copyWith(isClocking: true, error: null));

    final result = await _timeRecordService.clockOut();

    result.fold(
      (failure) => emit(state.copyWith(
        isClocking: false,
        error: failure.message,
      )),
      (record) {
        add(HomeRefreshRequested());
      },
    );
  }

  Future<void> _onRefreshRequested(
    HomeRefreshRequested event,
    Emitter<HomeState> emit,
  ) async {
    final result = await _timeRecordService.getDailySummary();

    result.fold(
      (failure) => emit(state.copyWith(
        isClocking: false,
        error: failure.message,
      )),
      (summary) => emit(state.copyWith(
        isClocking: false,
        summary: summary,
        nextAction: _determineNextAction(summary),
      )),
    );
  }

  ClockAction _determineNextAction(DailySummary summary) {
    if (summary.records.isEmpty) {
      return ClockAction.clockIn;
    }

    final lastRecord = summary.records.last;
    return lastRecord.isClockIn ? ClockAction.clockOut : ClockAction.clockIn;
  }
}