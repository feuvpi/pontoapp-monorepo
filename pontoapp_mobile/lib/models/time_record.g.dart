// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'time_record.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TimeRecord _$TimeRecordFromJson(Map<String, dynamic> json) => TimeRecord(
  id: json['id'] as String,
  userId: json['userId'] as String,
  userName: json['userName'] as String,
  recordedAt: DateTime.parse(json['recordedAt'] as String),
  type: json['type'] as String,
  status: json['status'] as String,
  authenticationType: json['authenticationType'] as String,
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  notes: json['notes'] as String?,
);

Map<String, dynamic> _$TimeRecordToJson(TimeRecord instance) =>
    <String, dynamic>{
      'id': instance.id,
      'userId': instance.userId,
      'userName': instance.userName,
      'recordedAt': instance.recordedAt.toIso8601String(),
      'type': instance.type,
      'status': instance.status,
      'authenticationType': instance.authenticationType,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'notes': instance.notes,
    };

DailySummary _$DailySummaryFromJson(Map<String, dynamic> json) => DailySummary(
  date: DateTime.parse(json['date'] as String),
  userId: json['userId'] as String,
  userName: json['userName'] as String,
  records: (json['records'] as List<dynamic>)
      .map((e) => TimeRecord.fromJson(e as Map<String, dynamic>))
      .toList(),
  totalWorkedFormatted: json['totalWorkedFormatted'] as String,
  totalRecords: (json['totalRecords'] as num).toInt(),
  isComplete: json['isComplete'] as bool,
  currentStatus: json['currentStatus'] as String?,
);

Map<String, dynamic> _$DailySummaryToJson(DailySummary instance) =>
    <String, dynamic>{
      'date': instance.date.toIso8601String(),
      'userId': instance.userId,
      'userName': instance.userName,
      'records': instance.records,
      'totalWorkedFormatted': instance.totalWorkedFormatted,
      'totalRecords': instance.totalRecords,
      'isComplete': instance.isComplete,
      'currentStatus': instance.currentStatus,
    };

ClockRequest _$ClockRequestFromJson(Map<String, dynamic> json) => ClockRequest(
  deviceId: json['deviceId'] as String,
  authenticationType: json['authenticationType'] as String,
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  notes: json['notes'] as String?,
);

Map<String, dynamic> _$ClockRequestToJson(ClockRequest instance) =>
    <String, dynamic>{
      'deviceId': instance.deviceId,
      'authenticationType': instance.authenticationType,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'notes': instance.notes,
    };
