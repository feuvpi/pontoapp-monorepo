<script lang="ts">
	import { onMount } from 'svelte';
	import type { User } from '$lib/types';
	import PageHeader from '$lib/components/layout/page-header.svelte';
	import ProfileSettings from '$lib/components/features/settings/profile-settings.svelte';
	import PasswordSettings from '$lib/components/features/settings/password-settings.svelte';
	import CompanySettings from '$lib/components/features/settings/company-settings.svelte';
	import { User as UserIcon, Building2, Lock } from 'lucide-svelte';
	import { auth } from '$lib/stores/auth.svelte';

	type Tab = 'profile' | 'password' | 'company';

	let activeTab = $state<Tab>('profile');
	let currentUser = $state<User | null>(null);

	onMount(() => {
		const user = auth.user;
		currentUser = user;
	});

	const tabs = [
    { id: 'profile', label: 'Meu Perfil', icon: UserIcon, adminOnly: false },
    { id: 'password', label: 'Alterar Senha', icon: Lock, adminOnly: false },
    { id: 'company', label: 'Empresa', icon: Building2, adminOnly: true }

	] as const;

	const canViewTab = (tab: (typeof tabs)[number]) => {
		if (!tab.adminOnly) return true;
		return currentUser?.role === 'Admin';
	};
</script>

<svelte:head>
	<title>Configurações | PontoApp</title>
</svelte:head>

<div class="space-y-6">
	<!-- Header -->
	<PageHeader title="Configurações" description="Gerencie sua conta e preferências" />

	<!-- Tabs -->
	<div class="border-b border-border">
		<nav class="-mb-px flex gap-6">
			{#each tabs.filter(canViewTab) as tab}
				<button
					onclick={() => (activeTab = tab.id)}
					class="flex items-center gap-2 border-b-2 px-1 py-4 text-sm font-medium transition-colors {activeTab ===
					tab.id
						? 'border-primary text-primary'
						: 'border-transparent text-muted-foreground hover:border-border hover:text-foreground'}"
				>
					<tab.icon class="h-4 w-4" />
					{tab.label}
				</button>
			{/each}
		</nav>
	</div>

	<!-- Tab Content -->
	<div class="pb-8">
		<div class="max-w-2xl">
			{#if activeTab === 'profile'}
				<div class="rounded-lg border border-border bg-card p-6">
					<ProfileSettings />
				</div>
			{:else if activeTab === 'password'}
				<div class="rounded-lg border border-border bg-card p-6">
					<PasswordSettings />
				</div>
			{:else if activeTab === 'company'}
				<CompanySettings />
			{/if}
		</div>
	</div>
</div>