// lib/core/errors/failures.dart
import 'package:equatable/equatable.dart';

abstract class Failure extends Equatable {
  final String message;
  const Failure(this.message);

  @override
  List<Object?> get props => [message];
}

class ServerFailure extends Failure {
  final int? statusCode;
  const ServerFailure(super.message, {this.statusCode});

  @override
  List<Object?> get props => [message, statusCode];
}

class NetworkFailure extends Failure {
  const NetworkFailure() : super('Sem conexão com a internet');
}

class CacheFailure extends Failure {
  const CacheFailure() : super('Erro ao acessar dados locais');
}

class AuthFailure extends Failure {
  const AuthFailure(super.message);
}

class BiometricFailure extends Failure {
  const BiometricFailure(super.message);
}

class ValidationFailure extends Failure {
  const ValidationFailure(super.message);
}