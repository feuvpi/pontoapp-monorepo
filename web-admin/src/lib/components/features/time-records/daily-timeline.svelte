<script lang="ts">
	import type { TimeRecord } from '$lib/types/time-record';
	import { formatTime } from '$lib/utils/format';
	import { Clock, Coffee, LogIn, LogOut } from 'lucide-svelte';

	interface Props {
		records: TimeRecord[];
		userName: string;
	}

	let { records, userName }: Props = $props();

	// Sort records by time
	const sortedRecords = $derived(
		[...records].sort(
			(a, b) => new Date(a.recordedAt).getTime() - new Date(b.recordedAt).getTime()
		)
	);

	// Calculate total worked time
	const totalWorkedMinutes = $derived(() => {
		let total = 0;
		let lastClockIn: Date | null = null;

		for (const record of sortedRecords) {
			const time = new Date(record.recordedAt);

			if (record.type === 'ClockIn') {
				lastClockIn = time;
			} else if (record.type === 'ClockOut' && lastClockIn) {
				const diff = time.getTime() - lastClockIn.getTime();
				total += diff / 1000 / 60; // Convert to minutes
				lastClockIn = null;
			}
		}

		return Math.floor(total);
	});

	function formatDuration(minutes: number): string {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		return `${hours}h ${mins}min`;
	}

	function getIcon(type: string) {
		switch (type) {
			case 'ClockIn':
				return LogIn;
			case 'ClockOut':
				return LogOut;
			case 'BreakStart':
			case 'BreakEnd':
				return Coffee;
			default:
				return Clock;
		}
	}

	function getColor(type: string): string {
		switch (type) {
			case 'ClockIn':
				return 'text-success-500 bg-success-50 border-success-200';
			case 'ClockOut':
				return 'text-destructive bg-destructive/10 border-destructive/20';
			case 'BreakStart':
			case 'BreakEnd':
				return 'text-warning-500 bg-warning-50 border-warning-200';
			default:
				return 'text-muted-foreground bg-muted border-border';
		}
	}

	function getLabel(type: string): string {
		switch (type) {
			case 'ClockIn':
				return 'Entrada';
			case 'ClockOut':
				return 'Saída';
			case 'BreakStart':
				return 'Início Intervalo';
			case 'BreakEnd':
				return 'Fim Intervalo';
			default:
				return type;
		}
	}
</script>

<div class="space-y-4">
	<!-- Header -->
	<div class="flex items-center justify-between">
		<div>
			<h3 class="text-lg font-semibold text-foreground">{userName}</h3>
			<p class="text-sm text-muted-foreground">
				{sortedRecords.length} registro{sortedRecords.length !== 1 ? 's' : ''} no dia
			</p>
		</div>
		{#if totalWorkedMinutes() > 0}
			<div class="text-right">
				<p class="text-sm text-muted-foreground">Total Trabalhado</p>
				<p class="text-xl font-bold text-foreground">{formatDuration(totalWorkedMinutes())}</p>
			</div>
		{/if}
	</div>

	<!-- Timeline -->
	<div class="relative space-y-4 pl-8">
		<!-- Vertical Line -->
		<div class="absolute left-[15px] top-0 h-full w-0.5 bg-border"></div>

		{#each sortedRecords as record, index (record.id)}
			{@const Icon = getIcon(record.type)}
			<div class="relative flex items-start gap-4">
				<!-- Icon -->
				<div
					class="absolute -left-8 flex h-8 w-8 items-center justify-center rounded-full border-2 {getColor(
						record.type
					)}"
				>
			
                    <Icon class="h-4 w-4" />
				</div>

				<!-- Content -->
				<div class="flex-1 rounded-lg border border-border bg-card p-4">
					<div class="flex items-start justify-between">
						<div>
							<p class="font-medium text-foreground">{getLabel(record.type)}</p>
							<p class="mt-1 text-2xl font-bold text-foreground">
								{formatTime(record.recordedAt)}
							</p>
							{#if record.notes}
								<p class="mt-2 text-sm text-muted-foreground">{record.notes}</p>
							{/if}
						</div>

						<!-- Duration between records -->
						{#if index > 0}
							{@const prevRecord = sortedRecords[index - 1]}
							{@const diff =
								new Date(record.recordedAt).getTime() -
								new Date(prevRecord.recordedAt).getTime()}
							{@const minutes = Math.floor(diff / 1000 / 60)}
							<div class="text-right">
								<p class="text-xs text-muted-foreground">Intervalo</p>
								<p class="text-sm font-medium text-foreground">{formatDuration(minutes)}</p>
							</div>
						{/if}
					</div>
				</div>
			</div>
		{/each}

		{#if sortedRecords.length === 0}
			<div class="rounded-lg border border-dashed border-border p-8 text-center">
				<Clock class="mx-auto h-12 w-12 text-muted-foreground/50" />
				<p class="mt-2 text-sm text-muted-foreground">Nenhum registro neste dia</p>
			</div>
		{/if}
	</div>
</div>