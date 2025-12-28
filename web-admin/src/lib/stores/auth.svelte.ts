import { browser } from '$app/environment';
import type { User } from '$lib/types';
import { STORAGE_KEYS } from '$lib/utils/constants';

/**
 * Auth State usando Svelte 5 Runes
 */
class AuthState {
	user = $state<User | null>(null);
	isLoading = $state(true);

	// Derived states
	isAuthenticated = $derived(this.user !== null);
	isAdmin = $derived(this.user?.role === 'Admin');
	isManager = $derived(this.user?.role === 'Manager');
	isHR = $derived(this.user?.role === 'HR');
	mustChangePassword = $derived(this.user?.mustChangePassword ?? false);

	constructor() {
		// Restore user from localStorage on mount
		if (browser) {
			this.restoreUser();
			this.setupEventListeners();
		}
	}

	/**
	 * Restore user from localStorage
	 */
	private restoreUser(): void {
		try {
			const userJson = localStorage.getItem(STORAGE_KEYS.USER);
			if (userJson) {
				this.user = JSON.parse(userJson);
			}
		} catch (error) {
			console.error('Failed to restore user:', error);
			localStorage.removeItem(STORAGE_KEYS.USER);
		} finally {
			this.isLoading = false;
		}
	}

	/**
	 * Setup event listeners for cross-tab sync
	 */
	private setupEventListeners(): void {
		// Listen for logout events from API client
		window.addEventListener('auth:logout', () => {
			this.logout();
		});

		// Listen for storage events (cross-tab sync)
		window.addEventListener('storage', (e) => {
			if (e.key === STORAGE_KEYS.USER) {
				if (e.newValue) {
					try {
						this.user = JSON.parse(e.newValue);
					} catch {
						this.user = null;
					}
				} else {
					this.user = null;
				}
			}
		});
	}

	/**
	 * Set authenticated user
	 */
	setUser(user: User): void {
		this.user = user;
		this.isLoading = false;

		if (browser) {
			localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(user));
		}
	}

	/**
	 * Update user data (partial update)
	 */
	updateUser(updates: Partial<User>): void {
		if (!this.user) return;

		this.user = { ...this.user, ...updates };

		if (browser) {
			localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(this.user));
		}
	}

	/**
	 * Logout user
	 */
	logout(): void {
		this.user = null;
		this.isLoading = false;

		if (browser) {
			localStorage.removeItem(STORAGE_KEYS.USER);
		}
	}

	/**
	 * Set loading state
	 */
	setLoading(loading: boolean): void {
		this.isLoading = loading;
	}

	/**
	 * Check if user has specific role
	 */
	hasRole(role: string | string[]): boolean {
		if (!this.user) return false;
		
		const roles = Array.isArray(role) ? role : [role];
		return roles.includes(this.user.role);
	}

	/**
	 * Check if user can manage employees
	 */
	canManageEmployees(): boolean {
		return this.hasRole(['Admin', 'HR', 'Manager']);
	}

	/**
	 * Check if user can view reports
	 */
	canViewReports(): boolean {
		return this.hasRole(['Admin', 'HR', 'Manager']);
	}

	/**
	 * Check if user can manage settings
	 */
	canManageSettings(): boolean {
		return this.hasRole(['Admin']);
	}
}

// Export singleton instance
export const auth = new AuthState();