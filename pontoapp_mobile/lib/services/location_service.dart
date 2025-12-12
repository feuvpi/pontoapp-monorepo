// lib/core/services/location_service.dart
import 'package:dartz/dartz.dart';
import 'package:geolocator/geolocator.dart';
import 'package:pontoapp_mobile/core/errors/failures.dart';

class LocationService {
  Future<Either<Failure, Position>> getCurrentPosition() async {
    try {
      final permission = await _checkPermission();
      if (permission != LocationPermission.always &&
          permission != LocationPermission.whileInUse) {
        return const Left(ServerFailure('Permissão de localização negada'));
      }

      final position = await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.high,
          timeLimit: Duration(seconds: 10),
        ),
      );

      return Right(position);
    } catch (e) {
      return Left(ServerFailure('Erro ao obter localização: $e'));
    }
  }

  Future<LocationPermission> _checkPermission() async {
    final serviceEnabled = await Geolocator.isLocationServiceEnabled();
    if (!serviceEnabled) {
      return LocationPermission.denied;
    }

    var permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      permission = await Geolocator.requestPermission();
    }

    return permission;
  }

  Future<bool> hasPermission() async {
    final permission = await Geolocator.checkPermission();
    return permission == LocationPermission.always ||
        permission == LocationPermission.whileInUse;
  }

  Future<bool> requestPermission() async {
    final permission = await Geolocator.requestPermission();
    return permission == LocationPermission.always ||
        permission == LocationPermission.whileInUse;
  }
}