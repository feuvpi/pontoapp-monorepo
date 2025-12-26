// lib/services/biometric_service.dart
import 'package:dartz/dartz.dart';
import 'package:local_auth/local_auth.dart';
import 'package:pontoapp_mobile/core/errors/failures.dart';

class BiometricService {
  final LocalAuthentication _localAuth = LocalAuthentication();

  Future<bool> isAvailable() async {
    try {
      final canCheck = await _localAuth.canCheckBiometrics;
      final isSupported = await _localAuth.isDeviceSupported();
      
      if (!canCheck || !isSupported) {
        return false;
      }
      
      // Verificar se tem biometria CADASTRADA
      final availableBiometrics = await _localAuth.getAvailableBiometrics();
      return availableBiometrics.isNotEmpty;
    } catch (_) {
      return false;
    }
  }

  Future<List<BiometricType>> getAvailableTypes() async {
    try {
      return await _localAuth.getAvailableBiometrics();
    } catch (_) {
      return [];
    }
  }

  Future<Either<Failure, bool>> authenticate({
    String reason = 'Autentique-se para registrar o ponto',
  }) async {
    try {
      final isAvailable = await this.isAvailable();
      if (!isAvailable) {
        return const Right(true); // Skip se não disponível
      }

      final authenticated = await _localAuth.authenticate(
        localizedReason: reason,
        options: const AuthenticationOptions(
          stickyAuth: true,
          biometricOnly: false,
        ),
      );

      return Right(authenticated);
    } catch (e) {
      return const Right(true); // Skip em caso de erro
    }
  }
}