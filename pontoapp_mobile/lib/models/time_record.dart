// lib/models/time_record.dart
import 'package:json_annotation/json_annotation.dart';

part 'time_record.g.dart';

@JsonSerializable()
class TimeRecord {
  final String id;
  final String userId;
  final String userName;
  final DateTime recordedAt;
  final String type;
  final String status;
  final String authenticationType;
  final double? latitude;
  final double? longitude;
  final String? notes;

  TimeRecord({
    required this.id,
    required this.userId,
    required this.userName,
    required this.recordedAt,
    required this.type,
    required this.status,
    required this.authenticationType,
    this.latitude,
    this.longitude,
    this.notes,
  });

  factory TimeRecord.fromJson(Map<String, dynamic> json) =>
      _$TimeRecordFromJson(json);

  Map<String, dynamic> toJson() => _$TimeRecordToJson(this);

  bool get isClockIn => type == 'ClockIn';
  bool get isClockOut => type == 'ClockOut';
}

@JsonSerializable()
class DailySummary {
  final DateTime date;
  final String userId;
  final String userName;
  final List<TimeRecord> records;
  final String totalWorkedFormatted;
  final int totalRecords;
  final bool isComplete;
  final String? currentStatus;

  DailySummary({
    required this.date,
    required this.userId,
    required this.userName,
    required this.records,
    required this.totalWorkedFormatted,
    required this.totalRecords,
    required this.isComplete,
    this.currentStatus,
  });

  factory DailySummary.fromJson(Map<String, dynamic> json) =>
      _$DailySummaryFromJson(json);

  bool get isWorking => currentStatus == 'Trabalhando';
}

@JsonSerializable()
class ClockRequest {
  final String deviceId;
  final String authenticationType;
  final double? latitude;
  final double? longitude;
  final String? notes;

  ClockRequest({
    required this.deviceId,
    required this.authenticationType,
    this.latitude,
    this.longitude,
    this.notes,
  });

  Map<String, dynamic> toJson() => _$ClockRequestToJson(this);
}