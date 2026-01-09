import { z } from 'zod';

/**
 * Login schema
 */
export const loginSchema = z.object({
	email: z
		.string()
		.min(1, 'E-mail é obrigatório')
		.email('E-mail inválido')
		.toLowerCase(),
	password: z.string().min(1, 'Senha é obrigatória')
});

export type LoginForm = z.infer<typeof loginSchema>;

/**
 * Change password schema
 */
export const changePasswordSchema = z
	.object({
		currentPassword: z.string().min(1, 'Senha atual é obrigatória'),
		newPassword: z
			.string()
			.min(6, 'Senha deve ter no mínimo 6 caracteres')
			.regex(/[A-Z]/, 'Senha deve conter ao menos uma letra maiúscula')
			.regex(/[a-z]/, 'Senha deve conter ao menos uma letra minúscula')
			.regex(/[0-9]/, 'Senha deve conter ao menos um número'),
		confirmPassword: z.string().min(1, 'Confirmação de senha é obrigatória')
	})
	.refine((data) => data.newPassword === data.confirmPassword, {
		message: 'As senhas não conferem',
		path: ['confirmPassword']
	});

export type ChangePasswordForm = z.infer<typeof changePasswordSchema>;

/**
 * Create user schema
 */
/**
39 * Create user schema
 */
export const createUserSchema = z.object({
	fullName: z
		.string()
		.min(3, 'Nome deve ter no mínimo 3 caracteres')
		.max(100, 'Nome muito longo')
		.trim(),
	email: z
		.string()
		.min(1, 'E-mail é obrigatório')
		.email('E-mail inválido')
		.toLowerCase(),
	password: z
		.string()
		.min(6, 'Senha deve ter no mínimo 6 caracteres')
		.regex(/[A-Z]/, 'Senha deve conter ao menos uma letra maiúscula')
		.regex(/[a-z]/, 'Senha deve conter ao menos uma letra minúscula')
		.regex(/[0-9]/, 'Senha deve conter ao menos um número'),
	employeeCode: z.string().trim().optional(),
	department: z.string().trim().optional(),
	role: z.enum(['Admin', 'Manager', 'HR', 'Employee']).default('Employee'),
	hiredAt: z.string().optional()
});


export type CreateUserForm = z.infer<typeof createUserSchema>;

/**
 * Update user schema (partial)
 */
export const updateUserSchema = createUserSchema.partial();

export type UpdateUserForm = z.infer<typeof updateUserSchema>;

/**
 * Brazilian CPF validation
 */
function isValidCPF(cpf: string): boolean {
	const cleaned = cpf.replace(/\D/g, '');
	
	if (cleaned.length !== 11) return false;
	if (/^(\d)\1{10}$/.test(cleaned)) return false;
	
	let sum = 0;
	for (let i = 0; i < 9; i++) {
		sum += parseInt(cleaned.charAt(i)) * (10 - i);
	}
	let remainder = (sum * 10) % 11;
	if (remainder === 10 || remainder === 11) remainder = 0;
	if (remainder !== parseInt(cleaned.charAt(9))) return false;
	
	sum = 0;
	for (let i = 0; i < 10; i++) {
		sum += parseInt(cleaned.charAt(i)) * (11 - i);
	}
	remainder = (sum * 10) % 11;
	if (remainder === 10 || remainder === 11) remainder = 0;
	if (remainder !== parseInt(cleaned.charAt(10))) return false;
	
	return true;
}

/**
 * CPF schema with validation
 */
export const cpfSchema = z
	.string()
	.min(1, 'CPF é obrigatório')
	.transform((val) => val.replace(/\D/g, ''))
	.refine(isValidCPF, 'CPF inválido');

/**
 * Phone schema (Brazilian)
 */
export const phoneSchema = z
	.string()
	.min(1, 'Telefone é obrigatório')
	.transform((val) => val.replace(/\D/g, ''))
	.refine((val) => val.length === 10 || val.length === 11, 'Telefone inválido');
