/**
 * Application Constants
 */

export const APP_NAME = 'PontoApp';

export const USER_ROLES = {
	ADMIN: 'Admin',
	MANAGER: 'Manager',
	HR: 'HR',
	EMPLOYEE: 'Employee'
} as const;

export const USER_STATUS = {
	ACTIVE: 'Active',
	INACTIVE: 'Inactive',
	PENDING: 'Pending'
} as const;

export const RECORD_TYPES = {
	CLOCK_IN: 'ClockIn',
	CLOCK_OUT: 'ClockOut',
	BREAK_START: 'BreakStart',
	BREAK_END: 'BreakEnd'
} as const;

export const RECORD_STATUS = {
	VALID: 'Valid',
	PENDING: 'Pending',
	REJECTED: 'Rejected'
} as const;

/**
 * Date/Time formats (pt-BR)
 */
export const DATE_FORMATS = {
	SHORT: 'dd/MM/yyyy',
	LONG: "dd 'de' MMMM 'de' yyyy",
	TIME: 'HH:mm',
	DATETIME: 'dd/MM/yyyy HH:mm',
	API: "yyyy-MM-dd'T'HH:mm:ss"
} as const;

/**
 * Pagination defaults
 */
export const PAGINATION = {
	DEFAULT_PAGE_SIZE: 20,
	PAGE_SIZE_OPTIONS: [10, 20, 50, 100]
} as const;

/**
 * Local storage keys
 */
export const STORAGE_KEYS = {
	ACCESS_TOKEN: 'accessToken',
	REFRESH_TOKEN: 'refreshToken',
	USER: 'user',
	THEME: 'theme'
} as const;

/**
 * API endpoints
 */
export const API_ENDPOINTS = {
	AUTH: {
		LOGIN: '/auth/login',
		REFRESH: '/auth/refresh',
		LOGOUT: '/auth/logout',
		CHANGE_PASSWORD: '/auth/change-password'
	},
	USERS: {
		LIST: '/users',
		GET: (id: string) => `/users/${id}`,
		CREATE: '/users',
		UPDATE: (id: string) => `/users/${id}`,
		DELETE: (id: string) => `/users/${id}`,
		ACTIVATE: (id: string) => `/users/${id}/activate`,
		DEACTIVATE: (id: string) => `/users/${id}/deactivate`
	},
	TIME_RECORDS: {
		LIST: '/time-records',
		GET: (id: string) => `/time-records/${id}`,
		BY_USER: (userId: string) => `/time-records/user/${userId}`,
		CREATE: '/time-records',
		UPDATE: (id: string) => `/time-records/${id}`,
		DELETE: (id: string) => `/time-records/${id}`
	}
} as const;