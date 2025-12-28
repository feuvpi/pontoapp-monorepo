import { goto } from '$app/navigation';
import { browser } from '$app/environment';
import { PUBLIC_API_URL } from '$env/static/public';
import type { ApiError as ApiErrorType } from '$lib/types';
import { STORAGE_KEYS, API_ENDPOINTS } from '$lib/utils/constants';

/**
 * Custom API Error class
 */
export class ApiError extends Error implements ApiErrorType {
	constructor(
		message: string,
		public statusCode?: number,
		public errors?: Record<string, string[]>
	) {
		super(message);
		this.name = 'ApiError';
	}
}

/**
 * API Client options
 */
interface ApiOptions extends RequestInit {
	skipAuth?: boolean;
	skipErrorToast?: boolean;
}

/**
 * Main API Client
 */
class ApiClient {
	private baseUrl: string;
	private accessToken: string | null = null;
	private refreshToken: string | null = null;
	private refreshPromise: Promise<boolean> | null = null;

	constructor() {
		this.baseUrl = PUBLIC_API_URL;

		// Restore tokens from localStorage on browser
		if (browser) {
			this.accessToken = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
			this.refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
		}
	}

	/**
	 * Set authentication tokens
	 */
	setTokens(access: string, refresh: string): void {
		this.accessToken = access;
		this.refreshToken = refresh;

		if (browser) {
			localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, access);
			localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refresh);
		}
	}

	/**
	 * Clear authentication tokens
	 */
	clearTokens(): void {
		this.accessToken = null;
		this.refreshToken = null;

		if (browser) {
			localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
			localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
			localStorage.removeItem(STORAGE_KEYS.USER);
		}
	}

	/**
	 * Get current access token
	 */
	getAccessToken(): string | null {
		return this.accessToken;
	}

	/**
	 * Main request handler
	 */
	private async request<T>(endpoint: string, options: ApiOptions = {}): Promise<T> {
		const { skipAuth = false, skipErrorToast = false, ...fetchOptions } = options;

		// Build headers
		const headers: Record<string, string> = {
			'Content-Type': 'application/json'
		};

		// Merge with existing headers
		if (fetchOptions.headers) {
			const existingHeaders = new Headers(fetchOptions.headers);
			existingHeaders.forEach((value, key) => {
				headers[key] = value;
			});
		}

		// Add auth header if not skipped
		if (!skipAuth && this.accessToken) {
			headers['Authorization'] = `Bearer ${this.accessToken}`;
		}

		try {
			const response = await fetch(`${this.baseUrl}${endpoint}`, {
				...fetchOptions,
				headers
			});

			// Handle 401 - Unauthorized (try refresh token)
			if (response.status === 401 && !skipAuth && this.refreshToken) {
				const refreshed = await this.ensureValidToken();

				if (refreshed) {
					// Retry original request with new token
					headers['Authorization'] = `Bearer ${this.accessToken}`;
					const retryResponse = await fetch(`${this.baseUrl}${endpoint}`, {
						...fetchOptions,
						headers
					});
					return this.handleResponse<T>(retryResponse, skipErrorToast);
				} else {
					// Refresh failed - logout
					this.handleAuthError();
					throw new ApiError('Sessão expirada. Faça login novamente.', 401);
				}
			}

			return this.handleResponse<T>(response, skipErrorToast);
		} catch (error) {
			// Network errors
			if (error instanceof TypeError && error.message === 'Failed to fetch') {
				const networkError = new ApiError('Erro de conexão. Verifique sua internet.', 0);
				if (!skipErrorToast) {
					this.showErrorToast(networkError.message);
				}
				throw networkError;
			}

			throw error;
		}
	}

	/**
	 * Handle API response
	 */
	private async handleResponse<T>(response: Response, skipErrorToast: boolean): Promise<T> {
		const contentType = response.headers.get('content-type');
		const isJson = contentType?.includes('application/json');

		// Parse response
		let data: unknown;
		if (isJson) {
			data = await response.json();
		} else {
			data = await response.text();
		}

		// Handle errors
		if (!response.ok) {
			const message =
				(data as { message?: string })?.message || `Erro ${response.status}`;
			const errors = (data as { errors?: Record<string, string[]> })?.errors;

			const apiError = new ApiError(message, response.status, errors);

			if (!skipErrorToast) {
				this.showErrorToast(message);
			}

			throw apiError;
		}

		// Extract data from wrapper if present
		if (
			data &&
			typeof data === 'object' &&
			'success' in data &&
			'data' in data
		) {
			return (data as { data: T }).data;
		}

		return data as T;
	}

	/**
	 * Ensure token is valid (with queue support)
	 */
	private async ensureValidToken(): Promise<boolean> {
		// If already refreshing, wait for it
		if (this.refreshPromise) {
			return this.refreshPromise;
		}

		// Start refresh
		this.refreshPromise = this.tryRefreshToken().finally(() => {
			this.refreshPromise = null;
		});

		return this.refreshPromise;
	}

	/**
	 * Try to refresh access token
	 */
	private async tryRefreshToken(): Promise<boolean> {
		if (!this.refreshToken) return false;

		try {
			const response = await fetch(`${this.baseUrl}${API_ENDPOINTS.AUTH.REFRESH}`, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({ refreshToken: this.refreshToken })
			});

			if (response.ok) {
				const data = (await response.json()) as {
					token: string;
					refreshToken: string;
				};
				this.setTokens(data.token, data.refreshToken);
				return true;
			}

			return false;
		} catch {
			return false;
		}
	}

	/**
	 * Handle authentication errors (logout)
	 */
	private handleAuthError(): void {
		this.clearTokens();

		// Dispatch custom event for auth store to listen
		if (browser) {
			window.dispatchEvent(new CustomEvent('auth:logout'));
			goto('/login');
		}
	}

	/**
	 * Show error toast (will be implemented with svelte-sonner)
	 */
	private showErrorToast(message: string): void {
		if (browser) {
			// Import dynamically to avoid SSR issues
			import('svelte-sonner').then(({ toast }) => {
				toast.error(message);
			});
		}
	}

	// ==================== HTTP Methods ====================

	/**
	 * GET request
	 */
	async get<T>(endpoint: string, options?: ApiOptions): Promise<T> {
		return this.request<T>(endpoint, { ...options, method: 'GET' });
	}

	/**
	 * POST request
	 */
	async post<T>(endpoint: string, body?: unknown, options?: ApiOptions): Promise<T> {
		return this.request<T>(endpoint, {
			...options,
			method: 'POST',
			body: body ? JSON.stringify(body) : undefined
		});
	}

	/**
	 * PUT request
	 */
	async put<T>(endpoint: string, body?: unknown, options?: ApiOptions): Promise<T> {
		return this.request<T>(endpoint, {
			...options,
			method: 'PUT',
			body: body ? JSON.stringify(body) : undefined
		});
	}

	/**
	 * PATCH request
	 */
	async patch<T>(endpoint: string, body?: unknown, options?: ApiOptions): Promise<T> {
		return this.request<T>(endpoint, {
			...options,
			method: 'PATCH',
			body: body ? JSON.stringify(body) : undefined
		});
	}

	/**
	 * DELETE request
	 */
	async delete<T>(endpoint: string, options?: ApiOptions): Promise<T> {
		return this.request<T>(endpoint, { ...options, method: 'DELETE' });
	}
}

// Export singleton instance
export const api = new ApiClient();