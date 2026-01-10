<script lang="ts">
	import type { User } from '$lib/types';
	import { timeRecordsService } from '$lib/services/time-records.service';
	import { RECORD_TYPE_LABELS } from '$lib/utils/constants';
	import { toast } from 'svelte-sonner';
	import { X } from 'lucide-svelte';
	import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
	import type { TimeRecord } from '$lib/types/time-record';

	interface Props {
		open: boolean;
		record?: TimeRecord | null;
		users: User[];
		onclose: () => void;
		onsuccess: (record: TimeRecord) => void;
	}

	let { open = $bindable(), record = null, users, onclose, onsuccess }: Props = $props();

	const isEditing = $derived(!!record);
	const title = $derived(isEditing ? 'Editar Registro' : 'Adicionar Registro Manual');

	let form = $state({
		userId: '',
		recordType: 'ClockIn',
		recordedAt: '',
		notes: ''
	});

	let errors = $state<Record<string, string>>({});
	let isSubmitting = $state(false);

	// Reset form when modal opens/closes
	$effect(() => {
		if (open && record) {
			// Extract date and time for datetime-local input
			const date = new Date(record.recordedAt);
			const localDateTime = new Date(date.getTime() - date.getTimezoneOffset() * 60000)
				.toISOString()
				.slice(0, 16);

			form = {
				userId: record.userId,
				recordType: record.type,
				recordedAt: localDateTime,
				notes: record.notes || ''
			};
		} else if (open && !record) {
			// Default to current date/time
			const now = new Date();
			const localDateTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
				.toISOString()
				.slice(0, 16);

			form = {
				userId: users[0]?.id || '',
				recordType: 'ClockIn',
				recordedAt: localDateTime,
				notes: ''
			};
		}
		errors = {};
	});

	async function handleSubmit(e: Event) {
		e.preventDefault();
		errors = {};

		// Validate
		if (!form.userId) errors.userId = 'Selecione um funcionário';
		if (!form.recordedAt) errors.recordedAt = 'Data/hora é obrigatória';

		if (Object.keys(errors).length > 0) return;

		isSubmitting = true;

		try {
			let resultRecord: TimeRecord;

			if (isEditing && record) {
				// Update
				resultRecord = await timeRecordsService.update(record.id, {
					recordType: form.recordType as any,
					recordedAt: new Date(form.recordedAt).toISOString(),
					notes: form.notes || undefined
				});
				toast.success('Registro atualizado com sucesso');
			} else {
				// Create manual
				resultRecord = await timeRecordsService.createManual({
					userId: form.userId,
					recordType: form.recordType as any,
					recordedAt: new Date(form.recordedAt).toISOString(),
					notes: form.notes || undefined
				});
				toast.success('Registro criado com sucesso');
			}

			onsuccess(resultRecord);
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
			onkeydown={(e) => e.key === 'Escape' && handleClose()}
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
				<!-- User Selection (only for create) -->
				{#if !isEditing}
					<div class="space-y-2">
						<label for="userId" class="text-sm font-medium text-foreground">
							Funcionário *
						</label>
						<select
							id="userId"
							bind:value={form.userId}
							class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
							class:border-destructive={errors.userId}
							disabled={isSubmitting}
						>
							<option value="">Selecione...</option>
							{#each users as user}
								<option value={user.id}>{user.fullName}</option>
							{/each}
						</select>
						{#if errors.userId}
							<p class="text-sm text-destructive">{errors.userId}</p>
						{/if}
					</div>
				{:else}
					<div class="space-y-2">
						<label for="userNameDisplay" class="text-sm font-medium text-foreground">Funcionário</label>
						<input
							id="userNameDisplay"
							type="text"
							value={record?.userName}
							disabled
							class="flex h-10 w-full rounded-md border border-input bg-muted px-3 py-2 text-sm"
						/>
					</div>
				{/if}

				<!-- Record Type -->
				<div class="space-y-2">
					<label for="recordType" class="text-sm font-medium text-foreground"> Tipo * </label>
					<select
						id="recordType"
						bind:value={form.recordType}
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						disabled={isSubmitting}
					>
						{#each Object.entries(RECORD_TYPE_LABELS) as [value, label]}
							<option {value}>{label}</option>
						{/each}
					</select>
				</div>

				<!-- Date/Time -->
				<div class="space-y-2">
					<label for="recordedAt" class="text-sm font-medium text-foreground">
						Data e Hora *
					</label>
					<input
						id="recordedAt"
						type="datetime-local"
						bind:value={form.recordedAt}
						class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						class:border-destructive={errors.recordedAt}
						disabled={isSubmitting}
					/>
					{#if errors.recordedAt}
						<p class="text-sm text-destructive">{errors.recordedAt}</p>
					{/if}
				</div>

				<!-- Notes -->
				<div class="space-y-2">
					<label for="notes" class="text-sm font-medium text-foreground"> Observações </label>
					<textarea
						id="notes"
						bind:value={form.notes}
						placeholder="Motivo do registro manual, ajuste, etc..."
						rows="3"
						class="flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50"
						disabled={isSubmitting}
					></textarea>
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
							<span>{isEditing ? 'Salvar Alterações' : 'Criar Registro'}</span>
						{/if}
					</button>
				</div>
			</form>
		</div>
	</div>
{/if}