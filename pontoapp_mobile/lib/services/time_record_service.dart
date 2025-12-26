// lib/services/time_record_service.dart
import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:pontoapp_mobile/core/network/api_client.dart';
import 'package:pontoapp_mobile/services/biometric_service.dart';
import 'package:pontoapp_mobile/services/device_info_service.dart';
import 'package:pontoapp_mobile/services/location_service.dart';
import 'package:pontoapp_mobile/core/errors/failures.dart';
import 'package:pontoapp_mobile/models/time_record.dart';

class TimeRecordService {
  final ApiClient _api;
  final BiometricService _biometric;
  final DeviceInfoService _deviceInfo;
  final LocationService _location;

  TimeRecordService(
    this._api,
    this._biometric,
    this._deviceInfo,
    this._location,
  );

  Future<Either<Failure, TimeRecord>> clockIn({bool useBiometric = true}) async {
    try {
      String authType = 'Password';
      
      if (useBiometric && await _biometric.isAvailable()) {
        final authResult = await _biometric.authenticate();
        final success = authResult.getOrElse(() => false);
        if (!success) {
          return const Left(BiometricFailure('Autenticação cancelada'));
        }
        authType = 'Biometric';
      }

      final deviceId = await _deviceInfo.getOrCreateDeviceId();

      double? lat, lng;
      final locResult = await _location.getCurrentPosition();
      locResult.fold((_) {}, (pos) {
        lat = pos.latitude;
        lng = pos.longitude;
      });

      final response = await _api.clockIn(ClockRequest(
        deviceId: deviceId,
        authenticationType: authType,
        latitude: lat,
        longitude: lng,
      ));

      if (response.success && response.data != null) {
        return Right(response.data!);
      } else {
        return Left(ServerFailure(response.message ?? 'Erro ao registrar entrada'));
      }
    } on DioException catch (e) {
      return Left(_handleDioError(e));
    } catch (e) {
      return Left(ServerFailure('Erro inesperado: $e'));
    }
  }

  Future<Either<Failure, TimeRecord>> clockOut({bool useBiometric = true}) async {
    try {
      String authType = 'Password';
      
      if (useBiometric && await _biometric.isAvailable()) {
        final authResult = await _biometric.authenticate();
        final success = authResult.getOrElse(() => false);
        if (!success) {
          return const Left(BiometricFailure('Autenticação cancelada'));
        }
        authType = 'Biometric';
      }

      final deviceId = await _deviceInfo.getOrCreateDeviceId();

      double? lat, lng;
      final locResult = await _location.getCurrentPosition();
      locResult.fold((_) {}, (pos) {
        lat = pos.latitude;
        lng = pos.longitude;
      });

      final response = await _api.clockOut(ClockRequest(
        deviceId: deviceId,
        authenticationType: authType,
        latitude: lat,
        longitude: lng,
      ));

      if (response.success && response.data != null) {
        return Right(response.data!);
      } else {
        return Left(ServerFailure(response.message ?? 'Erro ao registrar saída'));
      }
    } on DioException catch (e) {
      return Left(_handleDioError(e));
    } catch (e) {
      return Left(ServerFailure('Erro inesperado: $e'));
    }
  }

  Future<Either<Failure, DailySummary>> getDailySummary({DateTime? date}) async {
    try {
      final dateStr = date?.toIso8601String().split('T').first;
      final response = await _api.getDailySummary(dateStr);
      
      if (response.success && response.data != null) {
        return Right(response.data!);
      } else {
        return Left(ServerFailure(response.message ?? 'Erro ao buscar resumo'));
      }
    } on DioException catch (e) {
      return Left(_handleDioError(e));
    } catch (e) {
      return Left(ServerFailure('Erro inesperado: $e'));
    }
  }

  Future<Either<Failure, List<TimeRecord>>> getRecords({
    DateTime? startDate,
    DateTime? endDate,
  }) async {
    try {
      final response = await _api.getMyRecords(
        startDate?.toIso8601String(),
        endDate?.toIso8601String(),
      );
      
      if (response.success && response.data != null) {
        return Right(response.data!);
      } else {
        return Left(ServerFailure(response.message ?? 'Erro ao buscar registros'));
      }
    } on DioException catch (e) {
      return Left(_handleDioError(e));
    } catch (e) {
      return Left(ServerFailure('Erro inesperado: $e'));
    }
  }

  Failure _handleDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }

    final data = e.response?.data;
    String message = 'Erro no servidor';
    
    if (data is Map<String, dynamic>) {
      message = data['message'] ?? message;
    }
    
    final statusCode = e.response?.statusCode;

    if (statusCode == 401) {
      return const AuthFailure('Sessão expirada');
    }

    return ServerFailure(message, statusCode: statusCode);
  }
}