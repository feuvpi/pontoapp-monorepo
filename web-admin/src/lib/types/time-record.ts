/**
 * Time Record types
 */

export type RecordType = 'ClockIn' | 'ClockOut' | 'BreakStart' | 'BreakEnd';

export type RecordStatus = 'Valid' | 'Pending' | 'Rejected';

export interface Location {
	latitude: number;
	longitude: number;
	address?: string;
}

export interface TimeRecord {
	id: string;
	userId: string;
	userName?: string;
	user?: {
		id: string;
		fullName: string;
		employeeCode?: string;
	};
	type: string; // "ClockIn", "ClockOut", "BreakStart", "BreakEnd"
	recordedAt: string;
	status?: string; // "Valid", "Pending", "Rejected"
	authenticationType?: string; // "Password", "Biometric"
	latitude?: number | null;
	longitude?: number | null;
	deviceInfo?: string;
	notes?: string | null;
	createdAt: string;
	updatedAt?: string;
}

export interface CreateTimeRecordRequest {
	userId: string;
	recordType: RecordType;
	recordedAt: string;
	location?: Location;
	notes?: string;
}

export interface UpdateTimeRecordRequest {
	recordType?: RecordType;
	recordedAt?: string;
	location?: Location;
	notes?: string;
	status?: RecordStatus;
}

export interface TimeRecordFilters {
	userId?: string;
	startDate?: string;
	endDate?: string;
	recordType?: RecordType;
	status?: RecordStatus;
}

/**
 * Daily summary for a user
 */
export interface DailySummary {
	date: string;
	userId: string;
	userName: string;
	records: TimeRecord[];
	totalWorkedMinutes: number;
	clockIn?: string;
	clockOut?: string;
	isComplete: boolean;
}