<script lang="ts">
	let { children } = $props();
</script>

<div class="flex min-h-screen items-center justify-center bg-muted/30">
	<div class="w-full max-w-md">
		{@render children()}
	</div>
</div>