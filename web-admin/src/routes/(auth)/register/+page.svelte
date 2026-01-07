<script lang="ts">
	import { goto } from '$app/navigation';
	import { authService } from '$lib/services/auth.service';
	import { toast } from 'svelte-sonner';
	import { UserPlus } from 'lucide-svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
	import { z } from 'zod';

	// Validation schema
	const registerSchema = z
		.object({
			companyName: z.string().min(3, 'Nome da empresa muito curto').max(100),
			companyDocument: z
				.string()
				.min(1, 'CNPJ é obrigatório')
				.regex(/^\d{14}$|^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$/, 'CNPJ inválido'),
			adminName: z.string().min(3, 'Nome muito curto').max(100),
			adminEmail: z.string().email('E-mail inválido'),
			password: z
				.string()
				.min(6, 'Senha deve ter no mínimo 6 caracteres')
				.regex(/[A-Z]/, 'Deve conter letra maiúscula')
				.regex(/[a-z]/, 'Deve conter letra minúscula')
				.regex(/[0-9]/, 'Deve conter número'),
			confirmPassword: z.string()
		})
		.refine((data) => data.password === data.confirmPassword, {
			message: 'Senhas não conferem',
			path: ['confirmPassword']
		});

	// Form state
	let form = $state({
		companyName: '',
		companyDocument: '',
		adminName: '',
		adminEmail: '',
		password: '',
		confirmPassword: ''
	});

	let errors = $state<Record<string, string>>({});
	let isSubmitting = $state(false);

	// CNPJ mask
	function maskCNPJ(value: string) {
		return value
			.replace(/\D/g, '')
			.replace(/^(\d{2})(\d)/, '$1.$2')
			.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
			.replace(/\.(\d{3})(\d)/, '.$1/$2')
			.replace(/(\d{4})(\d)/, '$1-$2')
			.substring(0, 18);
	}

	function handleCNPJInput(e: Event) {
		const input = e.target as HTMLInputElement;
		input.value = maskCNPJ(input.value);
		form.companyDocument = input.value;
	}

	async function handleSubmit(e: Event) {
		e.preventDefault();
		errors = {};

		// Validate
		const result = registerSchema.safeParse(form);
		if (!result.success) {
			const fieldErrors = result.error.flatten().fieldErrors;
			errors = {
				companyName: fieldErrors.companyName?.[0] || '',
				companyDocument: fieldErrors.companyDocument?.[0] || '',
				adminName: fieldErrors.adminName?.[0] || '',
				adminEmail: fieldErrors.adminEmail?.[0] || '',
				password: fieldErrors.password?.[0] || '',
				confirmPassword: fieldErrors.confirmPassword?.[0] || ''
			};
			return;
		}

		isSubmitting = true;

		try {
			// Register returns LoginResponse with token
			const response = await authService.register({
				companyName: form.companyName,
				companyDocument: form.companyDocument.replace(/\D/g, ''), // Remove mask
				adminName: form.adminName,
				adminEmail: form.adminEmail,
				password: form.password
			});

			toast.success('Bem-vindo ao PontoApp! 🎉');
			goto('/dashboard');
		} catch (error: any) {
			console.error('Register error:', error);
		} finally {
			isSubmitting = false;
		}
	}
</script>

<svelte:head>
	<title>Cadastro | PontoApp</title>
</svelte:head>

<div class="space-y-6">
	<!-- Logo/Header -->
	<div class="text-center">
		<div class="mb-4 flex justify-center">
			<img src="/logo-icon.svg" alt="PontoApp" class="h-16 w-16" />
		</div>
		<h1 class="text-3xl font-bold tracking-tight text-foreground">Cadastre sua Empresa</h1>
		<p class="mt-2 text-sm text-muted-foreground">
			Comece a usar o PontoApp gratuitamente
		</p>
	</div>

	<!-- Register Card -->
	<div class="rounded-lg border border-border bg-card p-8 shadow-sm">
		<form onsubmit={handleSubmit} class="space-y-4">
			<!-- Company Info -->
			<div class="space-y-4">
				<h3 class="text-sm font-semibold text-foreground">Dados da Empresa</h3>

				<div class="space-y-2">
					<label for="companyName" class="text-sm font-medium text-foreground">
						Nome da Empresa *
					</label>
					<input
						id="companyName"
						type="text"
						bind:value={form.companyName}
						placeholder="Sua Empresa Ltda"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.companyName}
						disabled={isSubmitting}
					/>
					{#if errors.companyName}
						<p class="text-sm text-destructive">{errors.companyName}</p>
					{/if}
				</div>

				<div class="space-y-2">
					<label for="companyDocument" class="text-sm font-medium text-foreground">
						CNPJ *
					</label>
					<input
						id="companyDocument"
						type="text"
						value={form.companyDocument}
						oninput={handleCNPJInput}
						placeholder="00.000.000/0000-00"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.companyDocument}
						disabled={isSubmitting}
					/>
					{#if errors.companyDocument}
						<p class="text-sm text-destructive">{errors.companyDocument}</p>
					{/if}
				</div>
			</div>

			<div class="border-t border-border"></div>

			<!-- Admin Info -->
			<div class="space-y-4">
				<h3 class="text-sm font-semibold text-foreground">Administrador</h3>

				<div class="space-y-2">
					<label for="adminName" class="text-sm font-medium text-foreground">
						Nome Completo *
					</label>
					<input
						id="adminName"
						type="text"
						bind:value={form.adminName}
						placeholder="João da Silva"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.adminName}
						disabled={isSubmitting}
					/>
					{#if errors.adminName}
						<p class="text-sm text-destructive">{errors.adminName}</p>
					{/if}
				</div>

				<div class="space-y-2">
					<label for="adminEmail" class="text-sm font-medium text-foreground">
						E-mail *
					</label>
					<input
						id="adminEmail"
						type="email"
						bind:value={form.adminEmail}
						placeholder="admin@empresa.com.br"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.adminEmail}
						disabled={isSubmitting}
					/>
					{#if errors.adminEmail}
						<p class="text-sm text-destructive">{errors.adminEmail}</p>
					{/if}
				</div>

				<div class="grid grid-cols-2 gap-4">
					<div class="space-y-2">
						<label for="password" class="text-sm font-medium text-foreground">
							Senha *
						</label>
						<input
							id="password"
							type="password"
							bind:value={form.password}
							placeholder="••••••••"
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
							class:border-destructive={errors.password}
							disabled={isSubmitting}
						/>
						{#if errors.password}
							<p class="text-sm text-destructive">{errors.password}</p>
						{/if}
					</div>

					<div class="space-y-2">
						<label for="confirmPassword" class="text-sm font-medium text-foreground">
							Confirmar *
						</label>
						<input
							id="confirmPassword"
							type="password"
							bind:value={form.confirmPassword}
							placeholder="••••••••"
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
							class:border-destructive={errors.confirmPassword}
							disabled={isSubmitting}
						/>
						{#if errors.confirmPassword}
							<p class="text-sm text-destructive">{errors.confirmPassword}</p>
						{/if}
					</div>
				</div>

				<p class="text-xs text-muted-foreground">
					Senha deve ter: mínimo 6 caracteres, 1 maiúscula, 1 minúscula e 1 número
				</p>
			</div>

			<!-- Submit Button -->
			<button
				type="submit"
				disabled={isSubmitting}
				class="inline-flex h-10 w-full items-center justify-center gap-2 whitespace-nowrap rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50"
			>
				{#if isSubmitting}
					<LoadingSpinner size="sm" class="border-primary-foreground border-r-transparent" />
					<span>Criando conta...</span>
				{:else}
					<UserPlus class="h-4 w-4" />
					<span>Criar Conta Gratuitamente</span>
				{/if}
			</button>
		</form>
	</div>

	<!-- Login Link -->
	<p class="text-center text-sm text-muted-foreground">
		Já tem uma conta?
		<a href="/login" class="font-medium text-primary hover:underline">Fazer login</a>
	</p>
</div>