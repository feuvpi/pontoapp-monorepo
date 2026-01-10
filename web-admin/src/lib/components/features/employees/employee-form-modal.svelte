<script lang="ts">
	import type { User } from '$lib/types';
	import type { CreateUserForm } from '$lib/utils/validation';
	import { createUserSchema, updateUserSchema } from '$lib/utils/validation';
	import { USER_ROLE_LABELS } from '$lib/utils/constants';
	import { usersService } from '$lib/services/users.service';
	import { toast } from 'svelte-sonner';
	import { X } from 'lucide-svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';

	interface Props {
		open: boolean;
		user?: User | null;
		onclose: () => void;
		onsuccess: (user: User) => void;
	}

	let { open = $bindable(), user = null, onclose, onsuccess }: Props = $props();

	const isEditing = $derived(!!user);
	const title = $derived(isEditing ? 'Editar Funcionário' : 'Novo Funcionário');

	let form = $state<CreateUserForm>({
		fullName: '',
		email: '',
		password: '',
		employeeCode: '',
		department: '',
		role: 'Employee',
		hiredAt: ''
	});

	let errors = $state<Record<string, string>>({});
	let isSubmitting = $state(false);

	// Reset form when modal opens/closes or user changes
	$effect(() => {
		if (open && user) {
			form = {
				fullName: user.fullName,
				email: user.email,
				password: '', // Don't populate password
				employeeCode: user.employeeCode || '',
				department: user.department || '',
				role: user.role,
				hiredAt: user.hiredAt || ''
			};
		} else if (open && !user) {
			form = {
				fullName: '',
				email: '',
				password: '',
				employeeCode: '',
				department: '',
				role: 'Employee',
				hiredAt: ''
			};
		}
		errors = {};
	});

	async function handleSubmit(e: Event) {
		e.preventDefault();
		errors = {};

		// Validate
		const schema = isEditing ? updateUserSchema : createUserSchema;
		const result = schema.safeParse(form);

		if (!result.success) {
			const fieldErrors = result.error.flatten().fieldErrors;
			errors = Object.fromEntries(
				Object.entries(fieldErrors).map(([key, value]) => [key, value?.[0] || ''])
			);
			return;
		}

		isSubmitting = true;

		try {
			let resultUser: User;

			if (isEditing && user) {
				// Update
				const { password, ...updateData } = form;
				resultUser = await usersService.update(user.id, updateData);
				toast.success('Funcionário atualizado com sucesso');
			} else {
				// Create
				resultUser = await usersService.create(form as any);
				toast.success('Funcionário criado com sucesso');
			}

			onsuccess(resultUser);
			open = false;
		} catch (error: any) {
			console.error('Form error:', error);
		} finally {
			isSubmitting = false;
		}
	}

	function handleClose() {
		if (!isSubmitting) {
			open = false;
			onclose();
		}
	}
</script>

{#if open}
	<!-- Backdrop -->
	<div
		class="fixed inset-0 z-50 bg-background/80 backdrop-blur-sm"
		onclick={handleClose}
		role="presentation"
	></div>

	<!-- Modal -->
	<div class="fixed inset-0 z-50 flex items-center justify-center p-4">
		<div
			class="relative w-full max-w-lg rounded-lg border border-border bg-card p-6 shadow-lg"
			onclick={(e) => e.stopPropagation()}
			onkeydown={(e) => {
				if (e.key === 'Escape') {
					handleClose();
				}
			}}
			role="dialog"
			aria-modal="true"
			tabindex="0"
		>
			<!-- Header -->
			<div class="mb-4 flex items-center justify-between">
				<h2 class="text-xl font-semibold text-foreground">{title}</h2>
				<button
					onclick={handleClose}
					disabled={isSubmitting}
					class="rounded-sm opacity-70 ring-offset-background transition-opacity hover:opacity-100 disabled:pointer-events-none"
				>
					<X class="h-5 w-5" />
				</button>
			</div>

			<!-- Form -->
			<form onsubmit={handleSubmit} class="space-y-4">
				<!-- Full Name -->
				<div class="space-y-2">
					<label for="fullName" class="text-sm font-medium text-foreground">
						Nome Completo *
					</label>
					<input
						id="fullName"
						type="text"
						bind:value={form.fullName}
						placeholder="João da Silva"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.fullName}
						disabled={isSubmitting}
					/>
					{#if errors.fullName}
						<p class="text-sm text-destructive">{errors.fullName}</p>
					{/if}
				</div>

				<!-- Email -->
				<div class="space-y-2">
					<label for="email" class="text-sm font-medium text-foreground"> E-mail * </label>
					<input
						id="email"
						type="email"
						bind:value={form.email}
						placeholder="joao@empresa.com.br"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.email}
						disabled={isSubmitting}
					/>
					{#if errors.email}
						<p class="text-sm text-destructive">{errors.email}</p>
					{/if}
				</div>

				<!-- Password (only on create) -->
				{#if !isEditing}
					<div class="space-y-2">
						<label for="password" class="text-sm font-medium text-foreground">
							Senha Temporária *
						</label>
						<input
							id="password"
							type="password"
							bind:value={form.password}
							placeholder="••••••••"
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
							class:border-destructive={errors.password}
							disabled={isSubmitting}
						/>
						{#if errors.password}
							<p class="text-sm text-destructive">{errors.password}</p>
						{/if}
						<p class="text-xs text-muted-foreground">
							O funcionário deverá alterar a senha no primeiro login
						</p>
					</div>
				{/if}

				<!-- Employee Code & Department -->
				<div class="grid grid-cols-2 gap-4">
					<div class="space-y-2">
						<label for="employeeCode" class="text-sm font-medium text-foreground">
							Matrícula
						</label>
						<input
							id="employeeCode"
							type="text"
							bind:value={form.employeeCode}
							placeholder="EMP001"
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
							disabled={isSubmitting}
						/>
					</div>

					<div class="space-y-2">
						<label for="department" class="text-sm font-medium text-foreground">
							Departamento
						</label>
						<input
							id="department"
							type="text"
							bind:value={form.department}
							placeholder="TI"
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
							disabled={isSubmitting}
						/>
					</div>
				</div>

				<!-- Role & Hired At -->
				<div class="grid grid-cols-2 gap-4">
					<div class="space-y-2">
						<label for="role" class="text-sm font-medium text-foreground"> Cargo * </label>
						<select
							id="role"
							bind:value={form.role}
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
							disabled={isSubmitting}
						>
							{#each Object.entries(USER_ROLE_LABELS) as [value, label]}
								<option {value}>{label}</option>
							{/each}
						</select>
					</div>

					<div class="space-y-2">
						<label for="hiredAt" class="text-sm font-medium text-foreground">
							Data de Contratação
						</label>
						<input
							id="hiredAt"
							type="date"
							bind:value={form.hiredAt}
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
							disabled={isSubmitting}
						/>
					</div>
				</div>

				<!-- Actions -->
				<div class="flex justify-end gap-2 pt-4">
					<button
						type="button"
						onclick={handleClose}
						disabled={isSubmitting}
						class="inline-flex h-10 items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium ring-offset-background transition-colors hover:bg-accent hover:text-accent-foreground disabled:pointer-events-none disabled:opacity-50"
					>
						Cancelar
					</button>
					<button
						type="submit"
						disabled={isSubmitting}
						class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90 disabled:pointer-events-none disabled:opacity-50"
					>
						{#if isSubmitting}
							<LoadingSpinner size="sm" class="border-primary-foreground border-r-transparent" />
							<span>Salvando...</span>
						{:else}
							<span>{isEditing ? 'Salvar' : 'Criar funcionário'}</span>
						{/if}
					</button>
				</div>
			</form>
		</div>
	</div>
{/if}