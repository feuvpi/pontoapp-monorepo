<script lang="ts">
	import '../app.css';
	import { Toaster } from 'svelte-sonner';

	let { children } = $props();
</script>

{@render children()}

<!-- Toast notifications -->
<Toaster position="top-right" richColors />