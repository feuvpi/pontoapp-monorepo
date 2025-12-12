// lib/core/di/injection.dart
import 'package:get_it/get_it.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:pontoapp_mobile/core/network/dio_client.dart';
import 'package:pontoapp_mobile/core/network/api_client.dart';
import 'package:pontoapp_mobile/services/biometric_service.dart';
import 'package:pontoapp_mobile/services/device_info_service.dart';
import 'package:pontoapp_mobile/services/location_service.dart';
import 'package:pontoapp_mobile/services/auth_service.dart';
import 'package:pontoapp_mobile/services/time_record_service.dart';
import 'package:pontoapp_mobile/features/auth/bloc/auth_bloc.dart';

final getIt = GetIt.instance;

Future<void> configureDependencies() async {
  // 1. Storage (sem dependências)
  final storage = const FlutterSecureStorage();
  getIt.registerSingleton<FlutterSecureStorage>(storage);

  // 2. Network (depende de storage)
  final dioClient = DioClient(storage);
  getIt.registerSingleton<DioClient>(dioClient);
  getIt.registerSingleton<ApiClient>(ApiClient(dioClient.dio));

  // 3. Services básicos (sem dependências)
  final biometricService = BiometricService();
  getIt.registerSingleton<BiometricService>(biometricService);

  final locationService = LocationService();
  getIt.registerSingleton<LocationService>(locationService);

  // 4. DeviceInfoService (depende de storage e biometric)
  final deviceInfoService = DeviceInfoService(storage, biometricService);
  getIt.registerSingleton<DeviceInfoService>(deviceInfoService);

  // 5. App Services (dependem dos anteriores)
  final authService = AuthService(
    getIt<ApiClient>(),
    storage,
    deviceInfoService,
  );
  getIt.registerSingleton<AuthService>(authService);

  final timeRecordService = TimeRecordService(
    getIt<ApiClient>(),
    biometricService,
    deviceInfoService,
    locationService,
  );
  getIt.registerSingleton<TimeRecordService>(timeRecordService);

  // 6. BLoCs (factory - nova instância cada vez)
  getIt.registerFactory<AuthBloc>(() => AuthBloc(authService));
}