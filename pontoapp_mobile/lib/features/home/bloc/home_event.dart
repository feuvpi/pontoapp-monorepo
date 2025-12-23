

import 'package:equatable/equatable.dart';

abstract class HomeEvent extends Equatable {
  const HomeEvent();

  @override
  List<Object> get props => [];
}

class HomeLoadRequested extends HomeEvent {}

class HomeClockInRequested extends HomeEvent {}

class HomeClockOutRequested extends HomeEvent {}

class HomeRefreshRequested extends HomeEvent {}



