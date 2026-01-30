<script lang="ts">
	import { usersService } from '$lib/services/users.service';
	import type { User } from '$lib/types';
	import { toast } from 'svelte-sonner';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
	import { auth } from '$lib/stores/auth.svelte';

	let currentUser = $state<User | null>(null);
	let isLoading = $state(false);
	let isSaving = $state(false);

	let form = $state({
		fullName: '',
		email: '',
		employeeCode: '',
		department: ''
	});

	let errors = $state<Record<string, string>>({});

	// Load current user
	$effect(() => {
		const user = auth.user;
		if (user) {
			currentUser = user;
			form = {
				fullName: user.fullName,
				email: user.email,
				employeeCode: user.employeeCode || '',
				department: user.department || ''
			};
		}
	});

	async function handleSubmit(e: Event) {
		e.preventDefault();
		if (!currentUser) return;

		errors = {};

		// Validate
		if (!form.fullName) errors.fullName = 'Nome é obrigatório';
		if (!form.email) errors.email = 'E-mail é obrigatório';

		if (Object.keys(errors).length > 0) return;

		isSaving = true;

		try {
			const updatedUser = await usersService.update(currentUser.id, {
				fullName: form.fullName,
				email: form.email,
				employeeCode: form.employeeCode || undefined,
				department: form.department || undefined
			});

			// Update auth store
			auth.setUser(updatedUser);

			toast.success('Perfil atualizado com sucesso');
		} catch (error: any) {
			console.error('Update error:', error);
			toast.error('Erro ao atualizar perfil');
		} finally {
			isSaving = false;
		}
	}

	function handleCancel() {
		if (currentUser) {
			form = {
				fullName: currentUser.fullName,
				email: currentUser.email,
				employeeCode: currentUser.employeeCode || '',
				department: currentUser.department || ''
			};
			errors = {};
		}
	}
</script>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-semibold text-foreground">Informações Pessoais</h3>
		<p class="text-sm text-muted-foreground">
			Atualize suas informações de perfil e endereço de e-mail.
		</p>
	</div>

	<form onsubmit={handleSubmit} class="space-y-4">
		<!-- Full Name -->
		<div class="space-y-2">
			<label for="fullName" class="text-sm font-medium text-foreground"> Nome Completo * </label>
			<input
				id="fullName"
				type="text"
				bind:value={form.fullName}
				placeholder="João da Silva"
				class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
				class:border-destructive={errors.fullName}
				disabled={isSaving}
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
				disabled={isSaving}
			/>
			{#if errors.email}
				<p class="text-sm text-destructive">{errors.email}</p>
			{/if}
		</div>

		<!-- Employee Code -->
		<div class="space-y-2">
			<label for="employeeCode" class="text-sm font-medium text-foreground"> Matrícula </label>
			<input
				id="employeeCode"
				type="text"
				bind:value={form.employeeCode}
				placeholder="EMP001"
				class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
				disabled={isSaving}
			/>
		</div>

		<!-- Department -->
		<div class="space-y-2">
			<label for="department" class="text-sm font-medium text-foreground"> Departamento </label>
			<input
				id="department"
				type="text"
				bind:value={form.department}
				placeholder="TI"
				class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
				disabled={isSaving}
			/>
		</div>

		<!-- Role (read-only) -->
		{#if currentUser}
			<div class="space-y-2">
				<label for="role" class="text-sm font-medium text-foreground"> Cargo </label>
				<input
					id="role"
					type="text"
					value={currentUser.role === 'Admin'
						? 'Administrador'
						: currentUser.role === 'Manager'
							? 'Gerente'
							: currentUser.role === 'HR'
								? 'RH'
								: 'Funcionário'}
					disabled
					class="flex h-10 w-full rounded-md border border-input bg-muted px-3 py-2 text-sm text-muted-foreground"
				/>
				<p class="text-xs text-muted-foreground">
					O cargo não pode ser alterado. Entre em contato com o administrador.
				</p>
			</div>
		{/if}

		<!-- Actions -->
		<div class="flex justify-end gap-2 pt-4 border-t border-border">
			<button
				type="button"
				onclick={handleCancel}
				disabled={isSaving}
				class="inline-flex h-10 items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium ring-offset-background transition-colors hover:bg-accent hover:text-accent-foreground disabled:pointer-events-none disabled:opacity-50"
			>
				Cancelar
			</button>
			<button
				type="submit"
				disabled={isSaving}
				class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90 disabled:pointer-events-none disabled:opacity-50"
			>
				{#if isSaving}
					<LoadingSpinner size="sm" class="border-primary-foreground border-r-transparent" />
					<span>Salvando...</span>
				{:else}
					<span>Salvar Alterações</span>
				{/if}
			</button>
		</div>
	</form>
</div>