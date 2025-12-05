// lib/core/network/dio_client.dart
import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:pontoapp_mobile/core/config/env_config.dart';
import 'package:pontoapp_mobile/core/constants/storage_keys.dart';
import 'package:pretty_dio_logger/pretty_dio_logger.dart';

class DioClient {
  final FlutterSecureStorage _storage;
  late final Dio dio;

  DioClient(this._storage) {
    dio = Dio(
      BaseOptions(
        baseUrl: EnvConfig.baseUrl,
        connectTimeout: Duration(milliseconds: EnvConfig.connectTimeout),
        receiveTimeout: Duration(milliseconds: EnvConfig.receiveTimeout),
        headers: {'Content-Type': 'application/json'},
      ),
    );

    dio.interceptors.addAll([
      _AuthInterceptor(_storage),
      PrettyDioLogger(
        requestHeader: true,
        requestBody: true,
        responseBody: true,
        error: true,
        compact: true,
      ),
    ]);
  }
}

class _AuthInterceptor extends Interceptor {
  final FlutterSecureStorage _storage;

  _AuthInterceptor(this._storage);

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final token = await _storage.read(key: StorageKeys.accessToken);
    final tenantId = await _storage.read(key: StorageKeys.tenantId);

    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    if (tenantId != null) {
      options.headers['X-Tenant-Id'] = tenantId;
    }

    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    // TODO: Handle 401 - refresh token or logout
    handler.next(err);
  }
}