<script lang="ts">
	import { authService } from '$lib/services/auth.service';
	import { toast } from 'svelte-sonner';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';

	let form = $state({
		currentPassword: '',
		newPassword: '',
		confirmPassword: ''
	});

	let errors = $state<Record<string, string>>({});
	let isSaving = $state(false);

	async function handleSubmit(e: Event) {
		e.preventDefault();
		errors = {};

		// Validate
		if (!form.currentPassword) errors.currentPassword = 'Senha atual é obrigatória';
		if (!form.newPassword) errors.newPassword = 'Nova senha é obrigatória';
		if (form.newPassword.length < 6)
			errors.newPassword = 'Senha deve ter pelo menos 6 caracteres';
		if (form.newPassword !== form.confirmPassword)
			errors.confirmPassword = 'As senhas não conferem';

		if (Object.keys(errors).length > 0) return;

		isSaving = true;

		try {
			await authService.changePassword({
				currentPassword: form.currentPassword,
				newPassword: form.newPassword
			});

			toast.success('Senha alterada com sucesso');

			// Clear form
			form = {
				currentPassword: '',
				newPassword: '',
				confirmPassword: ''
			};
		} catch (error: any) {
			console.error('Change password error:', error);
			toast.error('Erro ao alterar senha');
		} finally {
			isSaving = false;
		}
	}
</script>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-semibold text-foreground">Alterar Senha</h3>
		<p class="text-sm text-muted-foreground">
			Atualize sua senha para manter sua conta segura.
		</p>
	</div>

	<form onsubmit={handleSubmit} class="space-y-4">
		<!-- Current Password -->
		<div class="space-y-2">
			<label for="currentPassword" class="text-sm font-medium text-foreground">
				Senha Atual *
			</label>
			<input
				id="currentPassword"
				type="password"
				bind:value={form.currentPassword}
				placeholder="••••••••"
				class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
				class:border-destructive={errors.currentPassword}
				disabled={isSaving}
			/>
			{#if errors.currentPassword}
				<p class="text-sm text-destructive">{errors.currentPassword}</p>
			{/if}
		</div>

		<!-- New Password -->
		<div class="space-y-2">
			<label for="newPassword" class="text-sm font-medium text-foreground"> Nova Senha * </label>
			<input
				id="newPassword"
				type="password"
				bind:value={form.newPassword}
				placeholder="••••••••"
				class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
				class:border-destructive={errors.newPassword}
				disabled={isSaving}
			/>
			{#if errors.newPassword}
				<p class="text-sm text-destructive">{errors.newPassword}</p>
			{/if}
			<p class="text-xs text-muted-foreground">Mínimo de 6 caracteres</p>
		</div>

		<!-- Confirm Password -->
		<div class="space-y-2">
			<label for="confirmPassword" class="text-sm font-medium text-foreground">
				Confirmar Nova Senha *
			</label>
			<input
				id="confirmPassword"
				type="password"
				bind:value={form.confirmPassword}
				placeholder="••••••••"
				class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
				class:border-destructive={errors.confirmPassword}
				disabled={isSaving}
			/>
			{#if errors.confirmPassword}
				<p class="text-sm text-destructive">{errors.confirmPassword}</p>
			{/if}
		</div>

		<!-- Actions -->
		<div class="flex justify-end gap-2 pt-4 border-t border-border">
			<button
				type="submit"
				disabled={isSaving}
				class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90 disabled:pointer-events-none disabled:opacity-50"
			>
				{#if isSaving}
					<LoadingSpinner size="sm" class="border-primary-foreground border-r-transparent" />
					<span>Alterando...</span>
				{:else}
					<span>Alterar Senha</span>
				{/if}
			</button>
		</div>
	</form>
</div>