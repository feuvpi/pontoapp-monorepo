<script lang="ts">
	import { Bell, Search } from 'lucide-svelte';
	import { cn } from '$lib/utils/cn';

	interface Props {
		class?: string;
	}

	let { class: className }: Props = $props();
</script>

<header
	class={cn(
		'sticky top-0 z-10 flex h-16 items-center justify-between border-b border-border bg-background px-6',
		className
	)}
>
	<!-- Search (future feature) -->
	<div class="flex flex-1 items-center gap-4">
		<div class="relative w-96">
			<Search class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
			<input
				type="search"
				placeholder="Buscar funcionários, registros..."
				class="h-10 w-full rounded-md border border-input bg-background pl-9 pr-4 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
				disabled
			/>
		</div>
	</div>

	<!-- Actions -->
	<div class="flex items-center gap-4">
		<!-- Notifications (future feature) -->
		<button
			class="relative rounded-md p-2 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
			disabled
		>
			<Bell class="h-5 w-5" />
			<!-- Notification badge -->
			<!-- <span class="absolute right-1 top-1 h-2 w-2 rounded-full bg-destructive"></span> -->
		</button>
	</div>
</header>