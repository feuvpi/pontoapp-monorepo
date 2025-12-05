// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'device.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Device _$DeviceFromJson(Map<String, dynamic> json) => Device(
  id: json['id'] as String,
  deviceId: json['deviceId'] as String,
  platform: json['platform'] as String,
  model: json['model'] as String?,
  osVersion: json['osVersion'] as String?,
  biometricCapable: json['biometricCapable'] as bool,
  isActive: json['isActive'] as bool,
  lastUsedAt: json['lastUsedAt'] == null
      ? null
      : DateTime.parse(json['lastUsedAt'] as String),
  createdAt: DateTime.parse(json['createdAt'] as String),
);

Map<String, dynamic> _$DeviceToJson(Device instance) => <String, dynamic>{
  'id': instance.id,
  'deviceId': instance.deviceId,
  'platform': instance.platform,
  'model': instance.model,
  'osVersion': instance.osVersion,
  'biometricCapable': instance.biometricCapable,
  'isActive': instance.isActive,
  'lastUsedAt': instance.lastUsedAt?.toIso8601String(),
  'createdAt': instance.createdAt.toIso8601String(),
};

RegisterDeviceRequest _$RegisterDeviceRequestFromJson(
  Map<String, dynamic> json,
) => RegisterDeviceRequest(
  deviceId: json['deviceId'] as String,
  platform: json['platform'] as String,
  model: json['model'] as String?,
  osVersion: json['osVersion'] as String?,
  biometricCapable: json['biometricCapable'] as bool,
  pushToken: json['pushToken'] as String?,
);

Map<String, dynamic> _$RegisterDeviceRequestToJson(
  RegisterDeviceRequest instance,
) => <String, dynamic>{
  'deviceId': instance.deviceId,
  'platform': instance.platform,
  'model': instance.model,
  'osVersion': instance.osVersion,
  'biometricCapable': instance.biometricCapable,
  'pushToken': instance.pushToken,
};
