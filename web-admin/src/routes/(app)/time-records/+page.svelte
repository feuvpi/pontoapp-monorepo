<script lang="ts">
	import { onMount } from 'svelte';
	import type { User } from '$lib/types';
	import { timeRecordsService } from '$lib/services/time-records.service';
	import { usersService } from '$lib/services/users.service';
	import PageHeader from '$lib/components/layout/page-header.svelte';
	import TimeRecordsTable from '$lib/components/features/time-records/time-records-table.svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
	import EmptyState from '$lib/components/shared/empty-state.svelte';
	import { Plus, Clock, Calendar, Filter, Download, Users } from 'lucide-svelte';
	import { toast } from 'svelte-sonner';
	import { formatDate } from '$lib/utils/format';
	import type { TimeRecord } from '$lib/types/time-record';
	import TimeRecordFormModal from '$lib/components/features/time-records/time-record-form-modal.svelte';

	let records = $state<TimeRecord[]>([]);
	let users = $state<User[]>([]);
	let isLoading = $state(true);
	let showModal = $state(false);
	let editingRecord = $state<TimeRecord | null>(null);

	// Filters
	let selectedUserId = $state<string>('');
	let startDate = $state<string>('');
	let endDate = $state<string>('');
	let selectedType = $state<string>('');
	let showFilters = $state(false);

	// Stats
	const stats = $derived({
		total: records.length,
		today: records.filter((r) => {
			const recordDate = new Date(r.recordedAt).toDateString();
			const today = new Date().toDateString();
			return recordDate === today;
		}).length,
		clockedIn: records.filter((r) => {
			const recordDate = new Date(r.recordedAt).toDateString();
			const today = new Date().toDateString();
			return recordDate === today && r.type === 'ClockIn';
		}).length,
		clockedOut: records.filter((r) => {
			const recordDate = new Date(r.recordedAt).toDateString();
			const today = new Date().toDateString();
			return recordDate === today && r.type === 'ClockOut';
		}).length
	});

	// Working now: users who have ClockIn but no ClockOut today
	const workingNow = $derived(() => {
		const today = new Date().toDateString();
		const todayRecords = records.filter(
			(r) => new Date(r.recordedAt).toDateString() === today
		);

		const userIds = new Set<string>();
		const working = new Set<string>();

		for (const record of todayRecords.sort(
			(a, b) => new Date(b.recordedAt).getTime() - new Date(a.recordedAt).getTime()
		)) {
			if (!userIds.has(record.userId)) {
				userIds.add(record.userId);
				if (record.type === 'ClockIn') {
					working.add(record.userId);
				}
			}
		}

		return working.size;
	});

	onMount(() => {
		loadData();
	});

	async function loadData() {
		isLoading = true;
		try {
			// Load users and records in parallel
			const [usersData, recordsData] = await Promise.all([
				usersService.getAll({ activeOnly: true }),
				loadRecords()
			]);

			users = usersData;
		} catch (error) {
			console.error('Failed to load data:', error);
		} finally {
			isLoading = false;
		}
	}

	async function loadRecords() {
		const filters: any = {};

		if (selectedUserId) filters.userId = selectedUserId;
		if (startDate) filters.startDate = new Date(startDate).toISOString();
		if (endDate) filters.endDate = new Date(endDate).toISOString();
		if (selectedType) filters.recordType = selectedType;

		records = await timeRecordsService.getAll(filters);
		return records;
	}

	function handleCreate() {
		editingRecord = null;
		showModal = true;
	}

	function handleEdit(event: CustomEvent<TimeRecord>) {
		editingRecord = event.detail;
		showModal = true;
	}

	function handleSuccess(record: TimeRecord) {
		if (editingRecord) {
			// Update existing
			records = records.map((r) => (r.id === record.id ? record : r));
		} else {
			// Add new
			records = [record, ...records];
		}
	}

	async function handleDelete(event: CustomEvent<TimeRecord>) {
		const record = event.detail;
		if (
			!confirm(
				`Deseja realmente deletar o registro de ${record.userName} (${formatDate(record.recordedAt)})?`
			)
		)
			return;

		try {
			await timeRecordsService.delete(record.id);
			records = records.filter((r) => r.id !== record.id);
			toast.success('Registro deletado');
		} catch (error) {
			console.error('Delete error:', error);
		}
	}

	async function handleApplyFilters() {
		isLoading = true;
		try {
			await loadRecords();
		} finally {
			isLoading = false;
		}
	}

	function handleClearFilters() {
		selectedUserId = '';
		startDate = '';
		endDate = '';
		selectedType = '';
		loadRecords();
	}

	function setTodayFilter() {
		const today = new Date();
		startDate = today.toISOString().split('T')[0];
		endDate = today.toISOString().split('T')[0];
		handleApplyFilters();
	}

	function setWeekFilter() {
		const today = new Date();
		const weekAgo = new Date(today);
		weekAgo.setDate(weekAgo.getDate() - 7);

		startDate = weekAgo.toISOString().split('T')[0];
		endDate = today.toISOString().split('T')[0];
		handleApplyFilters();
	}

	function setMonthFilter() {
		const today = new Date();
		const monthAgo = new Date(today);
		monthAgo.setMonth(monthAgo.getMonth() - 1);

		startDate = monthAgo.toISOString().split('T')[0];
		endDate = today.toISOString().split('T')[0];
		handleApplyFilters();
	}
</script>

<svelte:head>
	<title>Registros de Ponto | PontoApp</title>
</svelte:head>

<div class="space-y-6">
	<!-- Header -->
	<PageHeader
		title="Registros de Ponto"
		description="Visualize e gerencie todos os registros de ponto"
	>
		{#snippet children()}
			<div class="flex gap-2">
				<button
					onclick={() => (showFilters = !showFilters)}
					class="inline-flex h-10 items-center justify-center gap-2 rounded-md border border-input bg-background px-4 py-2 text-sm font-medium ring-offset-background transition-colors hover:bg-accent hover:text-accent-foreground"
				>
					<Filter class="h-4 w-4" />
					Filtros
				</button>
				<button
					onclick={handleCreate}
					class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground ring-offset-background transition-colors hover:bg-primary/90"
				>
					<Plus class="h-4 w-4" />
					Adicionar Registro
				</button>
			</div>
		{/snippet}
	</PageHeader>

	<!-- Stats Cards -->
	<div class="grid gap-4 md:grid-cols-4">
		<div class="rounded-lg border border-border bg-card p-4">
			<div class="flex items-center gap-2 text-muted-foreground">
				<Clock class="h-4 w-4" />
				<p class="text-sm">Total de Registros</p>
			</div>
			<p class="mt-2 text-2xl font-bold text-foreground">{stats.total}</p>
		</div>

		<div class="rounded-lg border border-border bg-card p-4">
			<div class="flex items-center gap-2 text-muted-foreground">
				<Calendar class="h-4 w-4" />
				<p class="text-sm">Registros Hoje</p>
			</div>
			<p class="mt-2 text-2xl font-bold text-foreground">{stats.today}</p>
		</div>

		<div class="rounded-lg border border-border bg-card p-4">
			<div class="flex items-center gap-2 text-success-500">
				<Users class="h-4 w-4" />
				<p class="text-sm">Trabalhando Agora</p>
			</div>
			<p class="mt-2 text-2xl font-bold text-success-500">{workingNow()}</p>
		</div>

		<div class="rounded-lg border border-border bg-card p-4">
			<div class="flex items-center gap-2 text-muted-foreground">
				<Download class="h-4 w-4" />
				<p class="text-sm">Saídas Hoje</p>
			</div>
			<p class="mt-2 text-2xl font-bold text-foreground">{stats.clockedOut}</p>
		</div>
	</div>

	<!-- Filters Panel -->
	{#if showFilters}
		<div class="rounded-lg border border-border bg-card p-4">
			<div class="mb-4 flex items-center justify-between">
				<h3 class="text-lg font-semibold text-foreground">Filtros</h3>
				<button
					onclick={handleClearFilters}
					class="text-sm text-primary hover:underline"
				>
					Limpar filtros
				</button>
			</div>

			<div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
				<!-- User Filter -->
				<div class="space-y-2">
					<label for="userFilter" class="text-sm font-medium text-foreground">
						Funcionário
					</label>
					<select
						id="userFilter"
						bind:value={selectedUserId}
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
					>
						<option value="">Todos</option>
						{#each users as user}
							<option value={user.id}>{user.fullName}</option>
						{/each}
					</select>
				</div>

				<!-- Start Date -->
				<div class="space-y-2">
					<label for="startDate" class="text-sm font-medium text-foreground">
						Data Inicial
					</label>
					<input
						id="startDate"
						type="date"
						bind:value={startDate}
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
					/>
				</div>

				<!-- End Date -->
				<div class="space-y-2">
					<label for="endDate" class="text-sm font-medium text-foreground"> Data Final </label>
					<input
						id="endDate"
						type="date"
						bind:value={endDate}
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
					/>
				</div>

				<!-- Type Filter -->
				<div class="space-y-2">
					<label for="typeFilter" class="text-sm font-medium text-foreground"> Tipo </label>
					<select
						id="typeFilter"
						bind:value={selectedType}
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm"
					>
						<option value="">Todos</option>
						<option value="ClockIn">Entrada</option>
						<option value="ClockOut">Saída</option>
						<option value="BreakStart">Início Intervalo</option>
						<option value="BreakEnd">Fim Intervalo</option>
					</select>
				</div>
			</div>

			<!-- Quick Filters -->
			<div class="mt-4 flex gap-2">
				<button
					onclick={setTodayFilter}
					class="inline-flex h-8 items-center justify-center rounded-md border border-input bg-background px-3 text-xs font-medium transition-colors hover:bg-accent"
				>
					Hoje
				</button>
				<button
					onclick={setWeekFilter}
					class="inline-flex h-8 items-center justify-center rounded-md border border-input bg-background px-3 text-xs font-medium transition-colors hover:bg-accent"
				>
					Últimos 7 dias
				</button>
				<button
					onclick={setMonthFilter}
					class="inline-flex h-8 items-center justify-center rounded-md border border-input bg-background px-3 text-xs font-medium transition-colors hover:bg-accent"
				>
					Últimos 30 dias
				</button>
			</div>

			<!-- Apply Button -->
			<div class="mt-4 flex justify-end">
				<button
					onclick={handleApplyFilters}
					class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
				>
					Aplicar Filtros
				</button>
			</div>
		</div>
	{/if}

	<!-- Content -->
	{#if isLoading}
		<div class="flex items-center justify-center py-12">
			<LoadingSpinner size="lg" />
		</div>
	{:else if records.length === 0}
		<EmptyState
			icon={Clock}
			title="Nenhum registro encontrado"
			description="Tente ajustar os filtros ou adicione um registro manual"
		>
			{#snippet children()}
				<button
					onclick={handleCreate}
					class="inline-flex h-10 items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90"
				>
					<Plus class="h-4 w-4" />
					Adicionar Registro
				</button>
			{/snippet}
		</EmptyState>
	{:else}
		<!-- Table -->
<TimeRecordsTable
	{records}
	on:edit={handleEdit}
	on:delete={handleDelete}
	on:viewLocation={(e: CustomEvent) => {
		const record = e.detail;
		if (record.latitude && record.longitude) {
			window.open(
				`https://www.google.com/maps?q=${record.latitude},${record.longitude}`,
				'_blank'
			);
		}
	}}
/>
	{/if}
</div>

<!-- Modal -->
<TimeRecordFormModal
	bind:open={showModal}
	record={editingRecord}
	{users}
	onclose={() => (showModal = false)}
	onsuccess={handleSuccess}
/>