import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:pontoapp_mobile/core/errors/failures.dart';
import 'package:pontoapp_mobile/models/user.dart';
import 'package:pontoapp_mobile/services/auth_service.dart';
import 'auth_event.dart';
import 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final AuthService _authService;

  AuthBloc(this._authService) : super(AuthInitial()) {
    on<AuthCheckRequested>(_onCheckRequested);
    on<AuthLoginRequested>(_onLoginRequested);
    on<AuthBiometricLoginRequested>(_onBiometricLoginRequested);
    on<AuthLogoutRequested>(_onLogoutRequested);
  }

  Future<void> _onCheckRequested(
    AuthCheckRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());

    final isAuthenticated = await _authService.isAuthenticated();

    if (isAuthenticated) {
      final userName = await _authService.getUserName() ?? 'Usuário';
      emit(AuthAuthenticated(userName: userName));
    } else {
      emit(AuthUnauthenticated());
    }
  }

  Future<void> _onLoginRequested(
    AuthLoginRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());

    final result = await _authService.login(event.email, event.password);

    result.fold(
      (Failure failure) => emit(AuthError(message: failure.message)),
      (User user) => emit(AuthAuthenticated(userName: user.fullName)),
    );
  }

  Future<void> _onBiometricLoginRequested(
    AuthBiometricLoginRequested event,
    Emitter<AuthState> emit,
  ) async {
    // TODO: Implement biometric login
    // This requires a saved session/token to work
    emit(const AuthError(message: 'Faça login primeiro para habilitar biometria'));
  }

  Future<void> _onLogoutRequested(
    AuthLogoutRequested event,
    Emitter<AuthState> emit,
  ) async {
    await _authService.logout();
    emit(AuthUnauthenticated());
  }
}