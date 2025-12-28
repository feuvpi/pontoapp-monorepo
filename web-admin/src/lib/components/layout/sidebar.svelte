<script lang="ts">
	import { page } from '$app/stores';
	import { auth } from '$lib/stores/auth.svelte';
	import { navigation, filterNavigationByRole } from '$lib/config/navigation';
	import { cn } from '$lib/utils/cn';
	import { getInitials } from '$lib/utils/format';
	import { ChevronDown, LogOut } from 'lucide-svelte';
	import { authService } from '$lib/services/auth.service';
	import { goto } from '$app/navigation';

	interface Props {
		class?: string;
	}

	let { class: className }: Props = $props();

	// Filter navigation based on user role
	const filteredNav = $derived(filterNavigationByRole(navigation, auth.user?.role));

	// User menu open state
	let userMenuOpen = $state(false);

	async function handleLogout() {
		await authService.logout();
		goto('/login');
	}
</script>

<aside
	class={cn(
		'flex h-screen w-64 flex-col border-r border-sidebar-border bg-sidebar',
		className
	)}
>
	<!-- Logo -->
	<div class="flex h-16 items-center border-b border-sidebar-border px-6">
		<h1 class="text-xl font-bold text-sidebar-foreground">PontoApp</h1>
	</div>

	<!-- Navigation -->
	<nav class="flex-1 space-y-1 overflow-y-auto px-3 py-4">
		{#each filteredNav as group}
			{#if group.title}
				<div class="mb-2 px-3 text-xs font-semibold uppercase text-sidebar-foreground/60">
					{group.title}
				</div>
			{/if}

			<div class="space-y-1">
				{#each group.items as item}
					{@const isActive = $page.url.pathname === item.href}
					<a
						href={item.href}
						class={cn(
							'group flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors',
							isActive
								? 'bg-sidebar-accent text-sidebar-accent-foreground'
								: 'text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-foreground'
						)}
					>
						<svelte:component this={item.icon} class="h-5 w-5 shrink-0" />
						<span class="flex-1">{item.title}</span>
						{#if item.badge}
							<span
								class="rounded-full bg-sidebar-primary px-2 py-0.5 text-xs font-medium text-sidebar-primary-foreground"
							>
								{item.badge}
							</span>
						{/if}
					</a>
				{/each}
			</div>

			{#if group !== filteredNav[filteredNav.length - 1]}
				<div class="my-2 border-t border-sidebar-border/50"></div>
			{/if}
		{/each}
	</nav>

	<!-- User section -->
	<div class="border-t border-sidebar-border p-3">
		<button
			onclick={() => (userMenuOpen = !userMenuOpen)}
			class={cn(
				'flex w-full items-center gap-3 rounded-md px-3 py-2 text-sm transition-colors',
				'hover:bg-sidebar-accent/50'
			)}
		>
			<!-- Avatar -->
			<div
				class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-sidebar-primary text-xs font-semibold text-sidebar-primary-foreground"
			>
				{getInitials(auth.user?.fullName || '')}
			</div>

			<!-- User info -->
			<div class="flex-1 text-left">
				<p class="font-medium text-sidebar-foreground">
					{auth.user?.fullName}
				</p>
				<p class="text-xs text-sidebar-foreground/60">
					{auth.user?.role}
				</p>
			</div>

			<ChevronDown
				class={cn(
					'h-4 w-4 text-sidebar-foreground/60 transition-transform',
					userMenuOpen && 'rotate-180'
				)}
			/>
		</button>

		{#if userMenuOpen}
			<div class="mt-2 space-y-1 rounded-md bg-sidebar-accent/30 p-1">
				<button
					onclick={handleLogout}
					class="flex w-full items-center gap-3 rounded px-3 py-2 text-sm text-sidebar-foreground/70 transition-colors hover:bg-sidebar-accent hover:text-sidebar-foreground"
				>
					<LogOut class="h-4 w-4" />
					<span>Sair</span>
				</button>
			</div>
		{/if}
	</div>
</aside>