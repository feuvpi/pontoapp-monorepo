// lib/core/network/api_client.dart
import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';

part 'api_client.g.dart';

@RestApi()
abstract class ApiClient {
  factory ApiClient(Dio dio, {String baseUrl}) = _ApiClient;

  // Auth
  @POST('/auth/login')
  Future<LoginResponse> login(@Body() LoginRequest request);

  // Time Records
  @POST('/timerecords/clock-in')
  Future<TimeRecord> clockIn(@Body() ClockRequest request);

  @POST('/timerecords/clock-out')
  Future<TimeRecord> clockOut(@Body() ClockRequest request);

  @GET('/timerecords/daily-summary')
  Future<DailySummary> getDailySummary(@Query('date') String? date);

  @GET('/timerecords/my-records')
  Future<List<TimeRecord>> getMyRecords(
    @Query('startDate') String? startDate,
    @Query('endDate') String? endDate,
  );

  // Devices
  @POST('/devices/register')
  Future<Device> registerDevice(@Body() RegisterDeviceRequest request);

  @GET('/devices/my-devices')
  Future<List<Device>> getMyDevices();

  @DELETE('/devices/{id}')
  Future<void> deactivateDevice(@Path('id') String id);
}