import 'package:dio/dio.dart';
import 'package:retrofit/retrofit.dart';
import 'package:pontoapp_mobile/models/user.dart';
import 'package:pontoapp_mobile/models/time_record.dart';
import 'package:pontoapp_mobile/models/device.dart';
import 'package:pontoapp_mobile/models/api_response.dart';

part 'api_client.g.dart';

@RestApi()
abstract class ApiClient {
  factory ApiClient(Dio dio, {String baseUrl}) = _ApiClient;

  // Auth
  @POST('/auth/login')
  Future<LoginResponse> login(@Body() LoginRequest request);

  // Time Records
  @POST('/timerecords/clock-in')
  Future<ApiResponse<TimeRecord>> clockIn(@Body() ClockRequest request);

  @POST('/timerecords/clock-out')
  Future<ApiResponse<TimeRecord>> clockOut(@Body() ClockRequest request);

  @GET('/timerecords/daily-summary')
  Future<ApiResponse<DailySummary>> getDailySummary(@Query('date') String? date);

  @GET('/timerecords/my-records')
  Future<ApiResponse<List<TimeRecord>>> getMyRecords(
    @Query('startDate') String? startDate,
    @Query('endDate') String? endDate,
  );

  // Devices
  @POST('/devices/register')
  Future<ApiResponse<Device>> registerDevice(@Body() RegisterDeviceRequest request);

  @GET('/devices/my-devices')
  Future<ApiResponse<List<Device>>> getMyDevices();

  @DELETE('/devices/{id}')
  Future<void> deactivateDevice(@Path('id') String id);
}