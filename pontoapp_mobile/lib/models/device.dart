// lib/models/device.dart
import 'package:json_annotation/json_annotation.dart';

part 'device.g.dart';

@JsonSerializable()
class Device {
  final String id;
  final String deviceId;
  final String platform;
  final String? model;
  final String? osVersion;
  final bool biometricCapable;
  final bool isActive;
  final DateTime? lastUsedAt;
  final DateTime createdAt;

  Device({
    required this.id,
    required this.deviceId,
    required this.platform,
    this.model,
    this.osVersion,
    required this.biometricCapable,
    required this.isActive,
    this.lastUsedAt,
    required this.createdAt,
  });

  factory Device.fromJson(Map<String, dynamic> json) => _$DeviceFromJson(json);
  Map<String, dynamic> toJson() => _$DeviceToJson(this);
}

@JsonSerializable()
class RegisterDeviceRequest {
  final String deviceId;
  final String platform;
  final String? model;
  final String? osVersion;
  final bool biometricCapable;
  final String? pushToken;

  RegisterDeviceRequest({
    required this.deviceId,
    required this.platform,
    this.model,
    this.osVersion,
    required this.biometricCapable,
    this.pushToken,
  });

  factory RegisterDeviceRequest.fromJson(Map<String, dynamic> json) =>
      _$RegisterDeviceRequestFromJson(json);
  Map<String, dynamic> toJson() => _$RegisterDeviceRequestToJson(this);
}