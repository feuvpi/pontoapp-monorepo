import { api } from './api';
import { auth } from '$lib/stores/auth.svelte';
import type { LoginResponse, RefreshTokenResponse } from '$lib/types';
import type { UserRole } from '$lib/types/user';
import { API_ENDPOINTS } from '$lib/utils/constants';

export interface LoginRequest {
	email: string;
	password: string;
}

export interface RegisterRequest {
	companyName: string;
	companyDocument: string;
	adminName: string;
	adminEmail: string;
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
	 * Register new company + admin user
	 */
	async register(data: RegisterRequest): Promise<LoginResponse> {
		const response = await api.post<LoginResponse>(
			'/Auth/register',
			data,
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
			isActive: true,
			createdAt: new Date().toISOString()
		});

		return response;
	},

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
			isActive: true,
			createdAt: new Date().toISOString()
		});

		return response;
	},

	/**
	 * Logout user
	 */
	async logout(): Promise<void> {
		try {
			// Call logout endpoint (opcional - .NET não tem endpoint de logout)
			// await api.post(API_ENDPOINTS.AUTH.LOGOUT, undefined, {
			// 	skipErrorToast: true
			// });
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
	async refreshToken(refreshToken: string): Promise<RefreshTokenResponse> {
		const response = await api.post<RefreshTokenResponse>(
			API_ENDPOINTS.AUTH.REFRESH,
			{ refreshToken },
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