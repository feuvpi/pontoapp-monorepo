/**
 * User types
 */

export type UserRole = 'Admin' | 'Manager' | 'HR' | 'Employee';

export type UserStatus = 'Active' | 'Inactive' | 'Pending';

export interface User {
	id: string;
	email: string;
	fullName: string;
	employeeCode?: string;
	department?: string;
	role: UserRole;
	status: UserStatus;
	mustChangePassword: boolean;
	createdAt: string;
	updatedAt: string;
	lastLoginAt?: string;
}

export interface CreateUserRequest {
	fullName: string;
	email: string;
	employeeCode?: string;
	department?: string;
	role?: UserRole;
}

export interface UpdateUserRequest {
	fullName?: string;
	email?: string;
	employeeCode?: string;
	department?: string;
	role?: UserRole;
}

export interface ChangePasswordRequest {
	currentPassword: string;
	newPassword: string;
}