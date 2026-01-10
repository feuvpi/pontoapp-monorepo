import { api } from './api';
import type {
	TimeRecord,
	CreateTimeRecordRequest,
	UpdateTimeRecordRequest,
	TimeRecordFilters,
	DailySummary
} from '$lib/types';

/**
 * Time Records Service
 */
export const timeRecordsService = {
	/**
	 * Get all time records (with filters)
	 */
	async getAll(filters?: TimeRecordFilters): Promise<TimeRecord[]> {
		const params = new URLSearchParams();

		if (filters?.userId) params.append('userId', filters.userId);
		if (filters?.startDate) params.append('startDate', filters.startDate);
		if (filters?.endDate) params.append('endDate', filters.endDate);
		if (filters?.recordType) params.append('recordType', filters.recordType);
		if (filters?.status) params.append('status', filters.status);

		const endpoint = `/TimeRecords${params.toString() ? `?${params.toString()}` : ''}`;
		return api.get<TimeRecord[]>(endpoint);
	},

	/**
	 * Get records for a specific user
	 */
	async getByUser(
		userId: string,
		startDate?: string,
		endDate?: string
	): Promise<TimeRecord[]> {
		const params = new URLSearchParams();
		if (startDate) params.append('startDate', startDate);
		if (endDate) params.append('endDate', endDate);

		const endpoint = `/TimeRecords/users/${userId}/records${params.toString() ? `?${params.toString()}` : ''}`;
		return api.get<TimeRecord[]>(endpoint);
	},

	/**
	 * Get daily summary for a user
	 */
	async getDailySummary(userId: string, date?: string): Promise<DailySummary> {
		const params = new URLSearchParams();
		if (date) params.append('date', date);

		const endpoint = `/TimeRecords/users/${userId}/daily-summary${params.toString() ? `?${params.toString()}` : ''}`;
		return api.get<DailySummary>(endpoint);
	},

	/**
	 * Create manual time record (admin)
	 */
	async createManual(data: CreateTimeRecordRequest): Promise<TimeRecord> {
		return api.post<TimeRecord>('/TimeRecords/manual', data);
	},

	/**
	 * Update time record (admin)
	 */
	async update(id: string, data: UpdateTimeRecordRequest): Promise<TimeRecord> {
		return api.put<TimeRecord>(`/TimeRecords/${id}`, data);
	},

	/**
	 * Delete time record (admin)
	 */
	async delete(id: string): Promise<void> {
		return api.delete(`/TimeRecords/${id}`);
	}
};