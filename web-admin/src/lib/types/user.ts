/**
 * User types
 */

export type UserRole = 'Admin' | 'Manager' | 'HR' | 'Employee';

export type UserStatus = 'Active' | 'Inactive' | 'Pending';

export interface User {
	id: string;
	fullName: string;
	email: string;
	role: UserRole;
	isActive: boolean;
	employeeCode?: string | null;
	department?: string | null;
	hiredAt?: string | null;
	createdAt: string;
	updatedAt?: string;
	lastLoginAt?: string;
	mustChangePassword?: boolean;
}

export interface CreateUserRequest {
	fullName: string;
	email: string;
	password: string;
	employeeCode?: string;
	department?: string;
	role?: UserRole;
	hiredAt?: string;
}

export interface UpdateUserRequest {
	fullName?: string;
	email?: string;
	employeeCode?: string;
	department?: string;
	role?: UserRole;
	hiredAt?: string;
}	

export interface ResetPasswordRequest {
	newPassword: string;
}


export interface ChangePasswordRequest {
	currentPassword: string;
	newPassword: string;
}