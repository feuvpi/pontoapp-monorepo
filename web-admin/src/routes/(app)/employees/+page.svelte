<script lang="ts">
	import { onMount } from 'svelte';
	import type { User } from '$lib/types';
	import { usersService } from '$lib/services/users.service';
	import { timeRecordsService } from '$lib/services/time-records.service';
	import PageHeader from '$lib/components/layout/page-header.svelte';
	import EmployeeTable from '$lib/components/features/employees/employee-table.svelte';
	import EmployeeFormModal from '$lib/components/features/employees/employee-form-modal.svelte';
	import DailyTimeline from '$lib/components/features/time-records/daily-timeline.svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
	import EmptyState from '$lib/components/shared/empty-state.svelte';
	import { Plus, Users, Search, X } from 'lucide-svelte';
	import { toast } from 'svelte-sonner';
	import type { TimeRecord } from '$lib/types/time-record';

	let users = $state<User[]>([]);
	let isLoading = $state(true);
	let searchTerm = $state('');
	let showModal = $state(false);
	let editingUser = $state<User | null>(null);
	let showResetPasswordModal = $state(false);
	let resetPasswordUser = $state<User | null>(null);
	let newPassword = $state('');

	// Timeline modal
	let showTimelineModal = $state(false);
	let timelineUser = $state<User | null>(null);
	let timelineRecords = $state<TimeRecord[]>([]);
	let loadingTimeline = $state(false);

	// Filtered users
	const filteredUsers = $derived(
		users.filter((user) => {
			const searchLower = searchTerm.toLowerCase();
			return (
				user.fullName.toLowerCase().includes(searchLower) ||
				user.email.toLowerCase().includes(searchLower) ||
				user.employeeCode?.toLowerCase().includes(searchLower) ||
				user.department?.toLowerCase().includes(searchLower)
			);
		})
	);

	onMount(() => {
		loadUsers();
	});

	async function loadUsers() {
		isLoading = true;
		try {
			users = await usersService.getAll();
		} catch (error) {
			console.error('Failed to load users:', error);
		} finally {
			isLoading = false;
		}
	}

	function handleCreate() {
		editingUser = null;
		showModal = true;
	}

	function handleEdit(event: CustomEvent<User>) {
		editingUser = event.detail;
		showModal = true;
	}

	function handleSuccess(user: User) {
		if (editingUser) {
			// Update existing
			users = users.map((u) => (u.id === user.id ? user : u));
		} else {
			// Add new
			users = [user, ...users];
		}
	}

	async function handleDeactivate(event: CustomEvent<User>) {
		const user = event.detail;
		if (!confirm(`Deseja realmente desativar ${user.fullName}?`)) return;

		try {
			await usersService.deactivate(user.id);
			users = users.map((u) => (u.id === user.id ? { ...u, isActive: false } : u));
			toast.success('Funcionário desativado');
		} catch (error) {
			console.error('Deactivate error:', error);
		}
	}

	async function handleActivate(event: CustomEvent<User>) {
		const user = event.detail;

		try {
			await usersService.activate(user.id);
			users = users.map((u) => (u.id === user.id ? { ...u, isActive: true } : u));
			toast.success('Funcionário reativado');
		} catch (error) {
			console.error('Activate error:', error);
		}
	}

	function handleResetPassword(event: CustomEvent<User>) {
		resetPasswordUser = event.detail;
		newPassword = '';
		showResetPasswordModal = true;
	}

	async function submitResetPassword() {
		if (!resetPasswordUser || !newPassword) return;

		try {
			await usersService.resetPassword(resetPasswordUser.id, { newPassword });
			toast.success('Senha resetada com sucesso');
			showResetPasswordModal = false;
			resetPasswordUser = null;
			newPassword = '';
		} catch (error) {
			console.error('Reset password error:', error);
		}
	}

	async function handleViewTimeline(event: CustomEvent<User>) {
		const user = event.detail;
		timelineUser = user;
		showTimelineModal = true;
		loadingTimeline = true;

		try {
			// Get today's records
			const today = new Date();
			const startOfDay = new Date(today.setHours(0, 0, 0, 0)).toISOString();
			const endOfDay = new Date(today.setHours(23, 59, 59, 999)).toISOString();

			timelineRecords = await timeRecordsService.getByUser(user.id, startOfDay, endOfDay);
		} catch (error) {
			console.error('Failed to load timeline:', error);
			toast.error('Erro ao carregar registros');
		} finally {
			loadingTimeline = false;
		}
	}
</script>

<svelte:head>
	<title>Funcionários | PontoApp</title>
</svelte:head>

<div class="space-y-6">
	<!-- Header -->
	<PageHeader title="Funcionários" description="Gerencie os funcionários da sua empresa">
		{#snippet children()}
			<button
				onclick={handleCreate}
				class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90"
			>
				<Plus class="h-4 w-4" />
				Novo Funcionário
			</button>
		{/snippet}
	</PageHeader>

	<!-- Search Bar -->
	<div class="flex items-center gap-4">
		<div class="relative flex-1 max-w-md">
			<Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
			<input
				type="search"
				placeholder="Buscar por nome, e-mail, matrícula..."
				bind:value={searchTerm}
				class="flex h-10 w-full rounded-md border border-input bg-background pl-9 pr-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
			/>
		</div>
	</div>

	<!-- Content -->
	{#if isLoading}
		<div class="flex items-center justify-center py-12">
			<LoadingSpinner size="lg" />
		</div>
	{:else if users.length === 0}
		<EmptyState
			icon={Users}
			title="Nenhum funcionário cadastrado"
			description="Comece adicionando o primeiro funcionário da sua empresa"
		>
			{#snippet children()}
				<button
					onclick={handleCreate}
					class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
				>
					<Plus class="h-4 w-4" />
					Adicionar Funcionário
				</button>
			{/snippet}
		</EmptyState>
	{:else}
		<!-- Stats -->
		<div class="grid gap-4 md:grid-cols-3">
			<div class="rounded-lg border border-border bg-card p-4">
				<p class="text-sm text-muted-foreground">Total</p>
				<p class="text-2xl font-bold text-foreground">{users.length}</p>
			</div>
			<div class="rounded-lg border border-border bg-card p-4">
				<p class="text-sm text-muted-foreground">Ativos</p>
				<p class="text-2xl font-bold text-success-500">
					{users.filter((u) => u.isActive).length}
				</p>
			</div>
			<div class="rounded-lg border border-border bg-card p-4">
				<p class="text-sm text-muted-foreground">Inativos</p>
				<p class="text-2xl font-bold text-muted-foreground">
					{users.filter((u) => !u.isActive).length}
				</p>
			</div>
		</div>

		<!-- Table -->
<EmployeeTable
	users={filteredUsers}
	on:edit={handleEdit}
	on:deactivate={handleDeactivate}
	on:activate={handleActivate}
	on:resetPassword={handleResetPassword}
	on:viewTimeline={handleViewTimeline}
/>
	{/if}
</div>

<!-- Modals -->
<EmployeeFormModal
	bind:open={showModal}
	user={editingUser}
	onclose={() => (showModal = false)}
	onsuccess={handleSuccess}
/>

<!-- Reset Password Modal -->
{#if showResetPasswordModal && resetPasswordUser}
	<div
		class="fixed inset-0 z-50 bg-background/80 backdrop-blur-sm"
		onclick={() => (showResetPasswordModal = false)}
		role="presentation"
	></div>
	<div class="fixed inset-0 z-50 flex items-center justify-center p-4">
		<div
			class="relative w-full max-w-md rounded-lg border border-border bg-card p-6"
			onclick={(e) => e.stopPropagation()}
		>
			<h2 class="mb-4 text-xl font-semibold">Resetar Senha</h2>
			<p class="mb-4 text-sm text-muted-foreground">
				Nova senha para <strong>{resetPasswordUser.fullName}</strong>
			</p>
			<input
				type="password"
				bind:value={newPassword}
				placeholder="Nova senha"
				class="mb-4 flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
			/>
			<div class="flex justify-end gap-2">
				<button
					onclick={() => (showResetPasswordModal = false)}
					class="inline-flex h-10 items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium"
				>
					Cancelar
				</button>
				<button
					onclick={submitResetPassword}
					class="inline-flex h-10 items-center justify-center rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground"
				>
					Resetar Senha
				</button>
			</div>
		</div>
	</div>
{/if}

<!-- Timeline Modal -->
{#if showTimelineModal && timelineUser}
	<div
		class="fixed inset-0 z-50 bg-background/80 backdrop-blur-sm"
		onclick={() => (showTimelineModal = false)}
		role="presentation"
	></div>
	<div class="fixed inset-0 z-50 flex items-center justify-center p-4">
		<div
			class="relative w-full max-w-2xl rounded-lg border border-border bg-card p-6 shadow-lg max-h-[80vh] overflow-y-auto"
			onclick={(e) => e.stopPropagation()}
		>
			<div class="mb-4 flex items-center justify-between">
				<h2 class="text-xl font-semibold">Registros de Hoje</h2>
				<button
					onclick={() => (showTimelineModal = false)}
					class="rounded-sm opacity-70 ring-offset-background transition-opacity hover:opacity-100"
				>
					<X class="h-5 w-5" />
				</button>
			</div>

			{#if loadingTimeline}
				<div class="flex items-center justify-center py-12">
					<LoadingSpinner size="lg" />
				</div>
			{:else}
				<DailyTimeline records={timelineRecords} userName={timelineUser.fullName} />
			{/if}
		</div>
	</div>
{/if}