/**
 * API Response types
 */

export interface ApiResponse<T = unknown> {
	success: boolean;
	message?: string;
	data?: T;
	errors?: Record<string, string[]>;
	timestamp: string;
}

export interface PaginatedResponse<T> {
	items: T[];
	totalCount: number;
	pageNumber: number;
	pageSize: number;
	totalPages: number;
	hasNextPage: boolean;
	hasPreviousPage: boolean;
}

export interface LoginResponse {
	token: string;
	refreshToken: string;
	expiresAt: string;
	user: {
		id: string;
		email: string;
		fullName: string;
		role: string;
		mustChangePassword: boolean;
	};
}

export interface RefreshTokenResponse {
	token: string;
	refreshToken: string;
	expiresAt: string;
}

/**
 * API Error types
 */
export class ApiError extends Error {
	constructor(
		message: string,
		public statusCode?: number,
		public errors?: Record<string, string[]>
	) {
		super(message);
		this.name = 'ApiError';
	}
}

export interface ValidationError {
	field: string;
	messages: string[];
}