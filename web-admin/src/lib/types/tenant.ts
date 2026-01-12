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