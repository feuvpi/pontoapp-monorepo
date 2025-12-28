import { api } from './api';
import { auth } from '$lib/stores/auth.svelte';
import type { LoginResponse, RefreshTokenResponse, UserRole } from '$lib/types';
import { API_ENDPOINTS } from '$lib/utils/constants';

export interface LoginRequest {
	email: string;
	password: string;
}

export interface ChangePasswordRequest {
	currentPassword: string;
	newPassword: string;
}

/**
 * Authentication Service
 */
export const authService = {
	/**
	 * Login user
	 */
	async login(credentials: LoginRequest): Promise<LoginResponse> {
		const response = await api.post<LoginResponse>(
			API_ENDPOINTS.AUTH.LOGIN,
			credentials,
			{ skipAuth: true }
		);

		// Store tokens
		api.setTokens(response.token, response.refreshToken);

		// Store user in auth state
		auth.setUser({
			id: response.user.id,
			email: response.user.email,
			fullName: response.user.fullName,
			role: response.user.role as UserRole,
			mustChangePassword: response.user.mustChangePassword,
			status: 'Active',
			createdAt: new Date().toISOString(),
			updatedAt: new Date().toISOString()
		});

		return response;
	},

	/**
	 * Logout user
	 */
	async logout(): Promise<void> {
		try {
			// Call logout endpoint (optional - server can invalidate token)
			await api.post(API_ENDPOINTS.AUTH.LOGOUT, undefined, {
				skipErrorToast: true
			});
		} catch {
			// Ignore errors on logout
		} finally {
			// Clear local state
			api.clearTokens();
			auth.logout();
		}
	},

	/**
	 * Change password
	 */
	async changePassword(data: ChangePasswordRequest): Promise<void> {
		await api.post(API_ENDPOINTS.AUTH.CHANGE_PASSWORD, data);

		// Update mustChangePassword flag
		auth.updateUser({ mustChangePassword: false });
	},

	/**
	 * Refresh access token
	 */
	async refreshToken(): Promise<RefreshTokenResponse> {
		const response = await api.post<RefreshTokenResponse>(
			API_ENDPOINTS.AUTH.REFRESH,
			{ refreshToken: api.getAccessToken() },
			{ skipAuth: true }
		);

		api.setTokens(response.token, response.refreshToken);

		return response;
	},

	/**
	 * Check if user is authenticated
	 */
	isAuthenticated(): boolean {
		return auth.isAuthenticated;
	},

	/**
	 * Get current user
	 */
	getCurrentUser() {
		return auth.user;
	}
};