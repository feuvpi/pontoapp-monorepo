<script lang="ts">
	import { onMount } from 'svelte';
	import { toast } from 'svelte-sonner';
	import { Building2 } from 'lucide-svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
	import { tenantService, type Tenant } from '$lib/services/tenant.service';

	let tenant = $state<Tenant | null>(null);
	let isLoading = $state(true);
	let isSaving = $state(false);

	let form = $state({
		name: '',
		email: '',
		companyDocument: ''
	});

	let errors = $state<Record<string, string>>({});

	onMount(() => {
		loadTenant();
	});

	async function loadTenant() {
		isLoading = true;
		try {
			tenant = await tenantService.getCurrent();
            console.log('Loaded tenant:', tenant);
			form = {
				name: tenant.name,
				email: tenant.email,
				companyDocument: tenant.companyDocument
			};
		} catch (error: any) {
			console.error('Failed to load tenant:', error);
			toast.error('Erro ao carregar dados da empresa');
		} finally {
			isLoading = false;
		}
	}

	async function handleSubmit(e: Event) {
		e.preventDefault();
		if (!tenant) return;

		errors = {};

		// Validate
		if (!form.name) errors.name = 'Nome é obrigatório';
		if (!form.email) errors.email = 'E-mail é obrigatório';
		if (!form.companyDocument) errors.companyDocument = 'CNPJ é obrigatório';

		if (Object.keys(errors).length > 0) return;

		isSaving = true;

		try {
			const updatedTenant = await tenantService.updateCurrent(form);
			tenant = updatedTenant;
			toast.success('Dados da empresa atualizados com sucesso');
		} catch (error: any) {
			console.error('Update error:', error);
			toast.error('Erro ao atualizar dados da empresa');
		} finally {
			isSaving = false;
		}
	}

	function handleCancel() {
		if (tenant) {
			form = {
				name: tenant.name,
				email: tenant.email,
				companyDocument: tenant.companyDocument
			};
			errors = {};
		}
	}

	function maskCNPJ(value: string): string {
		return value
			.replace(/\D/g, '')
			.replace(/^(\d{2})(\d)/, '$1.$2')
			.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
			.replace(/\.(\d{3})(\d)/, '.$1/$2')
			.replace(/(\d{4})(\d)/, '$1-$2')
			.substring(0, 18);
	}
</script>

<div class="space-y-6">
	<div>
		<h3 class="text-lg font-semibold text-foreground">Dados da Empresa</h3>
		<p class="text-sm text-muted-foreground">
			Gerencie as informações cadastrais da sua empresa.
		</p>
	</div>

	{#if isLoading}
		<div class="flex items-center justify-center py-12">
			<LoadingSpinner size="lg" />
		</div>
	{:else}
		<div class="rounded-lg border border-border bg-card p-6">
			<div class="flex items-center gap-3 mb-6">
				<div class="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10">
					<Building2 class="h-6 w-6 text-primary" />
				</div>
				<div>
					<h4 class="font-medium text-foreground">Sua Empresa</h4>
					<p class="text-sm text-muted-foreground">Informações cadastrais</p>
				</div>
			</div>

			<form onsubmit={handleSubmit} class="space-y-4">
				<!-- Company Name -->
				<div class="space-y-2">
					<label for="name" class="text-sm font-medium text-foreground">
						Razão Social *
					</label>
					<input
						id="name"
						type="text"
						bind:value={form.name}
						placeholder="Empresa Teste LTDA"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.name}
						disabled={isSaving}
					/>
					{#if errors.name}
						<p class="text-sm text-destructive">{errors.name}</p>
					{/if}
				</div>

				<!-- Email -->
				<div class="space-y-2">
					<label for="email" class="text-sm font-medium text-foreground">
						E-mail da Empresa *
					</label>
					<input
						id="email"
						type="email"
						bind:value={form.email}
						placeholder="contato@empresa.com.br"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.email}
						disabled={isSaving}
					/>
					{#if errors.email}
						<p class="text-sm text-destructive">{errors.email}</p>
					{/if}
				</div>

				<!-- CNPJ -->
				<div class="space-y-2">
					<label for="companyDocument" class="text-sm font-medium text-foreground">
						CNPJ *
					</label>
					<input
						id="companyDocument"
						type="text"
						value={maskCNPJ(form.companyDocument)}
						oninput={(e) => {
							const target = e.target as HTMLInputElement;
							form.companyDocument = target.value.replace(/\D/g, '');
						}}
						placeholder="00.000.000/0001-00"
						maxlength="18"
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.companyDocument}
						disabled={isSaving}
					/>
					{#if errors.companyDocument}
						<p class="text-sm text-destructive">{errors.companyDocument}</p>
					{/if}
				</div>

				<!-- Slug (read-only) -->
				{#if tenant}
					<div class="space-y-2">
						<label for="slug" class="text-sm font-medium text-foreground"> Identificador (Slug) </label>
						<input
							id="slug"
							type="text"
							value={tenant.slug}
							disabled
							class="flex h-10 w-full rounded-md border border-input bg-muted px-3 py-2 text-sm text-muted-foreground"
						/>
						<p class="text-xs text-muted-foreground">
							O identificador não pode ser alterado após a criação.
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
							<LoadingSpinner
								size="sm"
								class="border-primary-foreground border-r-transparent"
							/>
							<span>Salvando...</span>
						{:else}
							<span>Salvar Alterações</span>
						{/if}
					</button>
				</div>
			</form>
		</div>
	{/if}
</div>