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

export const USER_ROLE_LABELS: Record<string, string> = {
	Admin: 'Administrador',
	Manager: 'Gerente',
	HR: 'RH',
	Employee: 'Funcionário'
};

export const RECORD_TYPES = {
	CLOCK_IN: 'ClockIn',
	CLOCK_OUT: 'ClockOut',
	BREAK_START: 'BreakStart',
	BREAK_END: 'BreakEnd'
} as const;

export const RECORD_TYPE_LABELS: Record<string, string> = {
	ClockIn: 'Entrada',
	ClockOut: 'Saída',
	BreakStart: 'Início Intervalo',
	BreakEnd: 'Fim Intervalo'
};

export const RECORD_STATUS = {
	VALID: 'Valid',
	PENDING: 'Pending',
	REJECTED: 'Rejected'
} as const;

export const RECORD_STATUS_LABELS: Record<string, string> = {
	Valid: 'Válido',
	Pending: 'Pendente',
	Rejected: 'Rejeitado'
};

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
 * IMPORTANTE: A API usa rotas com MAIÚSCULA (Auth, Users, Tenants, TimeRecords)
 */
export const API_ENDPOINTS = {
	AUTH: {
		LOGIN: '/Auth/login',           // ← Era /auth/login
		REGISTER: '/Auth/register',
		REFRESH: '/Auth/refresh',
		LOGOUT: '/Auth/logout',
		CHANGE_PASSWORD: '/Auth/change-password'
	},
	USERS: {
		LIST: '/Users',                 // ← Era /users
		GET: (id: string) => `/Users/${id}`,
		CREATE: '/Users',
		UPDATE: (id: string) => `/Users/${id}`,
		DELETE: (id: string) => `/Users/${id}`,
		ACTIVATE: (id: string) => `/Users/${id}/activate`,
		RESET_PASSWORD: (id: string) => `/Users/${id}/reset-password`
	},
	TENANTS: {
		CURRENT: '/Tenants/current',    // ← Correto!
		UPDATE: '/Tenants/current'
	},
	TIME_RECORDS: {
		LIST: '/TimeRecords',           // ← Era /time-records
		GET: (id: string) => `/TimeRecords/${id}`,
		BY_USER: (userId: string) => `/TimeRecords/users/${userId}/records`,
		MANUAL: '/TimeRecords/manual',
		CREATE: '/TimeRecords',
		UPDATE: (id: string) => `/TimeRecords/${id}`,
		DELETE: (id: string) => `/TimeRecords/${id}`
	}
} as const;