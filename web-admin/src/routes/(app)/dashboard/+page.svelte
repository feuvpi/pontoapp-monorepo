<script lang="ts">
	import PageHeader from '$lib/components/layout/page-header.svelte';
	import { auth } from '$lib/stores/auth.svelte';
	import { Users, Clock, AlertCircle, TrendingUp } from 'lucide-svelte';

	// Placeholder stats
	const stats = [
		{
			title: 'Funcionários Ativos',
			value: '0',
			icon: Users,
			trend: '+0%'
		},
		{
			title: 'Trabalhando Agora',
			value: '0',
			icon: Clock,
			trend: '0 pessoas'
		},
		{
			title: 'Registros Hoje',
			value: '0',
			icon: TrendingUp,
			trend: '+0%'
		},
		{
			title: 'Pendências',
			value: '0',
			icon: AlertCircle,
			trend: '0 alertas'
		}
	];
</script>

<svelte:head>
	<title>Dashboard | PontoApp</title>
</svelte:head>

<div class="space-y-6">
	<PageHeader
		title="Bem-vindo, {auth.user?.fullName.split(' ')[0] || 'Usuário'}!"
		description="Visão geral do sistema de ponto"
	/>

	<!-- Stats Grid -->
	<div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
		{#each stats as stat}
			<div class="rounded-lg border border-border bg-card p-6">
				<div class="flex items-center justify-between">
					<p class="text-sm font-medium text-muted-foreground">{stat.title}</p>
					<svelte:component this={stat.icon} class="h-4 w-4 text-muted-foreground" />
				</div>
				<div class="mt-3">
					<p class="text-2xl font-bold text-foreground">{stat.value}</p>
					<p class="mt-1 text-xs text-muted-foreground">{stat.trend}</p>
				</div>
			</div>
		{/each}
	</div>

	<!-- Placeholder content -->
	<div class="rounded-lg border border-border bg-card p-12">
		<div class="text-center">
			<h3 class="text-lg font-semibold text-foreground">Dashboard em Desenvolvimento</h3>
			<p class="mt-2 text-sm text-muted-foreground">
				As estatísticas e gráficos serão implementados em breve.
			</p>
		</div>
	</div>
</div>