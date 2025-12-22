
import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:pontoapp_mobile/core/network/api_client.dart';
import 'package:pontoapp_mobile/core/constants/storage_keys.dart';
import 'package:pontoapp_mobile/core/errors/failures.dart';
import 'package:pontoapp_mobile/services/device_info_service.dart';
import 'package:pontoapp_mobile/models/user.dart';
import 'package:pontoapp_mobile/models/device.dart';

class AuthService {
  final ApiClient _api;
  final FlutterSecureStorage _storage;
  final DeviceInfoService _deviceInfo;

  AuthService(this._api, this._storage, this._deviceInfo);

  Future<Either<Failure, User>> login(String email, String password) async {
    try {
      final response = await _api.login(LoginRequest(
        email: email,
        password: password,
      ));

      // Salvar tokens
      await _storage.write(key: StorageKeys.accessToken, value: response.token);
      await _storage.write(key: StorageKeys.refreshToken, value: response.refreshToken);
      await _storage.write(key: StorageKeys.userId, value: response.user.id);
      await _storage.write(key: StorageKeys.tenantId, value: response.tenantId);
      await _storage.write(key: StorageKeys.userName, value: response.user.fullName);

      // Registrar device (silent)
      await _registerDevice();

      return Right(response.user);
    } on DioException catch (e) {
      return Left(_handleDioError(e));
    } catch (e) {
      return Left(ServerFailure('Erro inesperado: $e'));
    }
  }

  Future<void> _registerDevice() async {
    try {
      final request = RegisterDeviceRequest(
        deviceId: await _deviceInfo.getOrCreateDeviceId(),
        platform: await _deviceInfo.getPlatform(),
        model: await _deviceInfo.getModel(),
        osVersion: await _deviceInfo.getOsVersion(),
        biometricCapable: await _deviceInfo.isBiometricCapable(),
      );
      await _api.registerDevice(request);
    } catch (_) {
      // Silent fail
    }
  }

  Future<bool> isAuthenticated() async {
    final token = await _storage.read(key: StorageKeys.accessToken);
    return token != null;
  }

  Future<String?> getUserName() async {
    return await _storage.read(key: StorageKeys.userName);
  }

  Future<void> logout() async {
    await _storage.deleteAll();
  }

  Failure _handleDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }

    final statusCode = e.response?.statusCode;
    final data = e.response?.data;

    String message = 'Erro no servidor';
    if (data is Map<String, dynamic>) {
      message = data['message'] ?? data['title'] ?? message;
    }

    if (statusCode == 401) {
      return AuthFailure(message);
    }

    return ServerFailure(message, statusCode: statusCode);
  }
}