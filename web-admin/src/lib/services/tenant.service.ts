import { api } from './api';
import { API_ENDPOINTS } from '$lib/utils/constants';

export interface Tenant {
	id: string;
	name: string;
	slug: string;
	email: string;
	companyDocument: string;
	isActive: boolean;
	createdAt: string;
}

export interface UpdateTenantRequest {
	name: string;
	email: string;
	companyDocument: string;
}

/**
 * Tenant Service
 */
export const tenantService = {
	/**
	 * Get current tenant (logged user's company)
	 */
	async getCurrent(): Promise<Tenant> {
		return api.get<Tenant>(API_ENDPOINTS.TENANTS.CURRENT);
	},

	/**
	 * Update current tenant (Admin only)
	 */
	async updateCurrent(data: UpdateTenantRequest): Promise<Tenant> {
		return api.put<Tenant>(API_ENDPOINTS.TENANTS.UPDATE, data);
	}
};