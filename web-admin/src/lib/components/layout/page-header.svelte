<script lang="ts">
	import { cn } from '$lib/utils/cn';

	interface Props {
		title: string;
		description?: string;
		class?: string;
		children?: import('svelte').Snippet;
	}

	let { title, description, class: className, children }: Props = $props();
</script>

<div class={cn('flex items-center justify-between', className)}>
	<div class="space-y-1">
		<h1 class="text-3xl font-bold tracking-tight text-foreground">
			{title}
		</h1>
		{#if description}
			<p class="text-muted-foreground">
				{description}
			</p>
		{/if}
	</div>

	{#if children}
		<div class="flex items-center gap-2">
			{@render children()}
		</div>
	{/if}
</div>