// lib/core/services/device_info_service.dart
import 'dart:io';
import 'package:device_info_plus/device_info_plus.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:pontoapp_mobile/core/constants/storage_keys.dart';
import 'package:pontoapp_mobile/services/biometric_service.dart';
import 'package:uuid/uuid.dart';

class DeviceInfoService {
  final DeviceInfoPlugin _deviceInfo = DeviceInfoPlugin();
  final FlutterSecureStorage _storage;
  final BiometricService _biometricService;

  DeviceInfoService(this._storage, this._biometricService);

  Future<String> getOrCreateDeviceId() async {
    var deviceId = await _storage.read(key: StorageKeys.deviceId);
    if (deviceId == null) {
      deviceId = const Uuid().v4();
      await _storage.write(key: StorageKeys.deviceId, value: deviceId);
    }
    return deviceId;
  }

  Future<String> getPlatform() async {
    return Platform.isIOS ? 'iOS' : 'Android';
  }

  Future<String?> getModel() async {
    if (Platform.isAndroid) {
      final info = await _deviceInfo.androidInfo;
      return '${info.brand} ${info.model}';
    } else if (Platform.isIOS) {
      final info = await _deviceInfo.iosInfo;
      return info.utsname.machine;
    }
    return null;
  }

  Future<String?> getOsVersion() async {
    if (Platform.isAndroid) {
      final info = await _deviceInfo.androidInfo;
      return 'Android ${info.version.release}';
    } else if (Platform.isIOS) {
      final info = await _deviceInfo.iosInfo;
      return 'iOS ${info.systemVersion}';
    }
    return null;
  }

  Future<bool> isBiometricCapable() async {
    return await _biometricService.isAvailable();
  }
}