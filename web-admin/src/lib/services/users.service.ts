import { api } from './api';
import type { User, CreateUserRequest, UpdateUserRequest, ResetPasswordRequest } from '$lib/types';

export interface GetUsersParams {
	activeOnly?: boolean;
	role?: string;
}

/**
 * Users Service
 */
export const usersService = {
	/**
	 * Get all users
	 */
	async getAll(params?: GetUsersParams): Promise<User[]> {
		const queryParams = new URLSearchParams();
		
		if (params?.activeOnly !== undefined) {
			queryParams.append('activeOnly', params.activeOnly.toString());
		}
		
		if (params?.role) {
			queryParams.append('role', params.role);
		}

		const endpoint = `/Users${queryParams.toString() ? `?${queryParams.toString()}` : ''}`;
		return api.get<User[]>(endpoint);
	},

	/**
	 * Get user by ID
	 */
	async getById(id: string): Promise<User> {
		return api.get<User>(`/Users/${id}`);
	},

	/**
	 * Create new user
	 */
	async create(data: CreateUserRequest): Promise<User> {
		return api.post<User>('/Users', data);
	},

	/**
	 * Update user
	 */
	async update(id: string, data: UpdateUserRequest): Promise<User> {
		return api.put<User>(`/Users/${id}`, data);
	},

	/**
	 * Deactivate user (soft delete)
	 */
	async deactivate(id: string): Promise<void> {
		return api.delete(`/Users/${id}`);
	},

	/**
	 * Activate user
	 */
	async activate(id: string): Promise<void> {
		return api.post(`/Users/${id}/activate`);
	},

	/**
	 * Reset user password (Admin only)
	 */
	async resetPassword(id: string, data: ResetPasswordRequest): Promise<void> {
		return api.post(`/Users/${id}/reset-password`, data);
	}
};