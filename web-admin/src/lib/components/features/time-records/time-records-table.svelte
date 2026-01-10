<script lang="ts">
	import { formatDateTime, formatTime } from '$lib/utils/format';
	import { RECORD_TYPE_LABELS, RECORD_STATUS_LABELS } from '$lib/utils/constants';
	import { Edit, Trash2, MapPin } from 'lucide-svelte';
	import { createEventDispatcher } from 'svelte';
	import type { TimeRecord } from '$lib/types/time-record';

	interface Props {
		records: TimeRecord[];
		isLoading?: boolean;
	}

	let { records, isLoading = false }: Props = $props();

	const dispatch = createEventDispatcher<{
		edit: TimeRecord;
		delete: TimeRecord;
		viewLocation: TimeRecord;
	}>();

	function getTypeColor(type: string): string {
		switch (type) {
			case 'ClockIn':
				return 'bg-success-50 text-success-500';
			case 'ClockOut':
				return 'bg-destructive/10 text-destructive';
			case 'BreakStart':
				return 'bg-warning-50 text-warning-500';
			case 'BreakEnd':
				return 'bg-primary/10 text-primary';
			default:
				return 'bg-muted text-muted-foreground';
		}
	}

	function getStatusColor(status: string): string {
		switch (status) {
			case 'Valid':
				return 'bg-success-50 text-success-500';
			case 'Pending':
				return 'bg-warning-50 text-warning-500';
			case 'Rejected':
				return 'bg-destructive/10 text-destructive';
			default:
				return 'bg-muted text-muted-foreground';
		}
	}
</script>

<div class="rounded-lg border border-border bg-card">
	<div class="overflow-x-auto">
		<table class="w-full">
			<thead class="border-b border-border bg-muted/50">
				<tr>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Funcionário</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Data/Hora</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Tipo</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Status</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Autenticação</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Observações</th>
					<th class="px-4 py-3 text-right text-sm font-medium text-foreground">Ações</th>
				</tr>
			</thead>
			<tbody class="divide-y divide-border">
				{#if isLoading}
					<tr>
						<td colspan="7" class="px-4 py-8 text-center text-sm text-muted-foreground">
							Carregando...
						</td>
					</tr>
				{:else if records.length === 0}
					<tr>
						<td colspan="7" class="px-4 py-8 text-center text-sm text-muted-foreground">
							Nenhum registro encontrado
						</td>
					</tr>
				{:else}
					{#each records as record (record.id)}
						<tr class="hover:bg-muted/30 transition-colors">
							<td class="px-4 py-3 text-sm font-medium text-foreground">
								{record.userName}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								<div class="flex flex-col">
									<span class="font-medium text-foreground">
										{formatTime(record.recordedAt)}
									</span>
									<span class="text-xs">{formatDateTime(record.recordedAt).split(' ')[0]}</span>
								</div>
							</td>
							<td class="px-4 py-3">
								<span
									class="inline-flex items-center gap-1 rounded-full px-2 py-1 text-xs font-medium {getTypeColor(
										record.type
									)}"
								>
									{RECORD_TYPE_LABELS[record.type] || record.type}
								</span>
							</td>
							<td class="px-4 py-3">
								<span
									class="inline-flex items-center gap-1 rounded-full px-2 py-1 text-xs font-medium {getStatusColor(
										record.status || 'Valid'
									)}"
								>
									{RECORD_STATUS_LABELS[record.status || 'Valid'] || record.status}
								</span>
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{record.authenticationType === 'Biometric' ? 'Biometria' : 'Senha'}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{#if record.notes}
									<span class="truncate max-w-xs block" title={record.notes}>
										{record.notes}
									</span>
								{:else}
									<span class="text-muted-foreground/50">-</span>
								{/if}
							</td>
							<td class="px-4 py-3">
								<div class="flex items-center justify-end gap-2">
									{#if record.latitude && record.longitude}
										<button
											onclick={() => dispatch('viewLocation', record)}
											class="rounded p-1 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
											title="Ver localização"
										>
											<MapPin class="h-4 w-4" />
										</button>
									{/if}

									<button
										onclick={() => dispatch('edit', record)}
										class="rounded p-1 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
										title="Editar"
									>
										<Edit class="h-4 w-4" />
									</button>

									<button
										onclick={() => dispatch('delete', record)}
										class="rounded p-1 text-muted-foreground transition-colors hover:bg-destructive/10 hover:text-destructive"
										title="Deletar"
									>
										<Trash2 class="h-4 w-4" />
									</button>
								</div>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	</div>
</div>