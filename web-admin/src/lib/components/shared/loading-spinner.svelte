<script lang="ts">
	import { cn } from '$lib/utils/cn';

	interface Props {
		class?: string;
		size?: 'sm' | 'md' | 'lg';
	}

	let { class: className, size = 'md' }: Props = $props();

	const sizeClasses = {
		sm: 'h-4 w-4 border-2',
		md: 'h-8 w-8 border-2',
		lg: 'h-12 w-12 border-3'
	};
</script>

<div
	class={cn(
		'inline-block animate-spin rounded-full border-solid border-primary border-r-transparent',
		sizeClasses[size],
		className
	)}
	role="status"
	aria-label="Carregando"
>
	<span class="sr-only">Carregando...</span>
</div>