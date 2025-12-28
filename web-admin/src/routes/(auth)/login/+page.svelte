<script lang="ts">
	import { goto } from '$app/navigation';
	import { page } from '$app/stores';
	import { authService } from '$lib/services/auth.service';
	import { loginSchema, type LoginForm } from '$lib/utils/validation';
	import { toast } from 'svelte-sonner';
	import { LogIn } from 'lucide-svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';

	// Form state
	let form = $state<LoginForm>({
		email: '',
		password: ''
	});

	let errors = $state<Partial<Record<keyof LoginForm, string>>>({});
	let isSubmitting = $state(false);

	async function handleSubmit(e: Event) {
		e.preventDefault();
		errors = {};

		// Validate
		const result = loginSchema.safeParse(form);
		if (!result.success) {
			const fieldErrors = result.error.flatten().fieldErrors;
			errors = {
				email: fieldErrors.email?.[0],
				password: fieldErrors.password?.[0]
			};
			return;
		}

		isSubmitting = true;

		try {
			await authService.login(form);

			// Get redirect URL from query params or default to dashboard
			const redirectTo = $page.url.searchParams.get('redirectTo') || '/dashboard';

			toast.success('Login realizado com sucesso!');
			goto(redirectTo);
		} catch (error: any) {
			// Error toast already shown by API client
			console.error('Login error:', error);
		} finally {
			isSubmitting = false;
		}
	}
</script>

<svelte:head>
	<title>Login | PontoApp</title>
</svelte:head>

<div class="space-y-6">
	<!-- Logo/Header -->
	<div class="text-center">
		<h1 class="text-3xl font-bold tracking-tight text-foreground">PontoApp</h1>
		<p class="mt-2 text-sm text-muted-foreground">
			Entre com suas credenciais para acessar o sistema
		</p>
	</div>

	<!-- Login Card -->
	<div class="rounded-lg border border-border bg-card p-8 shadow-sm">
		<form onsubmit={handleSubmit} class="space-y-4">
			<!-- Email -->
			<div class="space-y-2">
				<label for="email" class="text-sm font-medium text-foreground">
					E-mail
				</label>
				<input
					id="email"
					type="email"
					bind:value={form.email}
					placeholder="seu@email.com"
					class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
					class:border-destructive={errors.email}
					disabled={isSubmitting}
				/>
				{#if errors.email}
					<p class="text-sm text-destructive">{errors.email}</p>
				{/if}
			</div>

			<!-- Password -->
			<div class="space-y-2">
				<div class="flex items-center justify-between">
					<label for="password" class="text-sm font-medium text-foreground">
						Senha
					</label>
					<a
						href="/forgot-password"
						class="text-sm text-primary hover:underline"
						tabindex="-1"
					>
						Esqueceu a senha?
					</a>
				</div>
				<input
					id="password"
					type="password"
					bind:value={form.password}
					placeholder="••••••••"
					class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
					class:border-destructive={errors.password}
					disabled={isSubmitting}
				/>
				{#if errors.password}
					<p class="text-sm text-destructive">{errors.password}</p>
				{/if}
			</div>

			<!-- Submit Button -->
			<button
				type="submit"
				disabled={isSubmitting}
				class="inline-flex h-10 w-full items-center justify-center gap-2 whitespace-nowrap rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50"
			>
				{#if isSubmitting}
					<LoadingSpinner size="sm" class="border-primary-foreground border-r-transparent" />
					<span>Entrando...</span>
				{:else}
					<LogIn class="h-4 w-4" />
					<span>Entrar</span>
				{/if}
			</button>
		</form>
	</div>

	<!-- Footer -->
	<p class="text-center text-sm text-muted-foreground">
		Desenvolvido por <span class="font-medium">PontoApp</span>
	</p>
</div>