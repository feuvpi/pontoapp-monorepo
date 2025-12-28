<script lang="ts">
	import Sidebar from '$lib/components/layout/sidebar.svelte';
	import Header from '$lib/components/layout/header.svelte';

	let { children } = $props();
</script>

<div class="flex h-screen overflow-hidden">
	<!-- Sidebar -->
	<Sidebar />

	<!-- Main content -->
	<div class="flex flex-1 flex-col overflow-hidden">
		<!-- Header -->
		<Header />

		<!-- Page content -->
		<main class="flex-1 overflow-y-auto bg-muted/30 p-6">
			{@render children()}
		</main>
	</div>
</div>