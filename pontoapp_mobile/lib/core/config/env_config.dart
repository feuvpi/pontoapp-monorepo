// lib/core/config/env_config.dart
abstract class EnvConfig {
  static const String baseUrl = String.fromEnvironment(
    'BASE_URL',
    defaultValue: 'http://localhost:5000/api/v1',
  );

  static const int connectTimeout = 30000;
  static const int receiveTimeout = 30000;
}