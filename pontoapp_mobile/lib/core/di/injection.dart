// lib/core/di/injection.dart
import 'package:get_it/get_it.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:pontoapp_mobile/core/network/dio_client.dart';
import 'package:pontoapp_mobile/services/biometric_service.dart';
import 'package:pontoapp_mobile/services/location_service.dart';

final getIt = GetIt.instance;

Future<void> configureDependencies() async {
  // Storage
  getIt.registerLazySingleton<FlutterSecureStorage>(
    () => const FlutterSecureStorage(),
  );

  // Network
  getIt.registerLazySingleton<DioClient>(
    () => DioClient(getIt<FlutterSecureStorage>()),
  );

  // Services
  getIt.registerLazySingleton<BiometricService>(
    () => BiometricService(),
  );

  getIt.registerLazySingleton<LocationService>(
    () => LocationService(),
  );
}