<script lang="ts">
	import type { ComponentType } from 'svelte';
	import { FileQuestion } from 'lucide-svelte';
	import { cn } from '$lib/utils/cn';

	interface Props {
		icon?: ComponentType;
		title: string;
		description?: string;
		class?: string;
		children?: import('svelte').Snippet;
	}

	let {
		icon: Icon = FileQuestion,
		title,
		description,
		class: className,
		children
	}: Props = $props();
</script>

<div class={cn('flex flex-col items-center justify-center py-12 text-center', className)}>
	<div class="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-muted">
		<Icon class="h-6 w-6 text-muted-foreground" />
	</div>

	<h3 class="mb-1 text-lg font-semibold text-foreground">
		{title}
	</h3>

	{#if description}
		<p class="mb-4 max-w-md text-sm text-muted-foreground">
			{description}
		</p>
	{/if}

	{#if children}
		<div class="mt-4">
			{@render children()}
		</div>
	{/if}
</div>