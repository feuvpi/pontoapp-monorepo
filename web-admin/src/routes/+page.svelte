<script lang="ts">
	import { Clock, Users, MapPin, Shield, Zap, TrendingUp, Check, ArrowRight, ChevronDown, Star, Building2, Play } from 'lucide-svelte';
	import { onMount } from 'svelte';

	let scrollY = $state(0);
	let isNavScrolled = $derived(scrollY > 50);
	let visibleSections = $state<Set<string>>(new Set());

	onMount(() => {
		const observer = new IntersectionObserver(
			(entries) => {
				entries.forEach((entry) => {
					if (entry.isIntersecting) {
						visibleSections = new Set([...visibleSections, entry.target.id]);
					}
				});
			},
			{ threshold: 0.1, rootMargin: '0px 0px -10% 0px' }
		);

		document.querySelectorAll('section[id]').forEach((section) => {
			observer.observe(section);
		});

		return () => observer.disconnect();
	});

	const stats = [
		{ value: '50mil+', label: 'Registros por mês' },
		{ value: '99.9%', label: 'Uptime garantido' },
		{ value: '500+', label: 'Empresas ativas' },
		{ value: '<2s', label: 'Tempo de registro' }
	];

	const testimonials = [
		{
			name: 'Carolina Mendes',
			role: 'Gerente de RH',
			company: 'TechStart Brasil',
			avatar: 'CM',
			content: 'Reduzimos em 80% o tempo gasto com controle de ponto. A integração com nossa folha de pagamento é perfeita.'
		},
		{
			name: 'Roberto Almeida',
			role: 'Diretor de Operações',
			company: 'Logística Express',
			avatar: 'RA',
			content: 'Com a geolocalização, conseguimos validar o ponto de mais de 200 motoristas em campo. Essencial para nossa operação.'
		},
		{
			name: 'Fernanda Costa',
			role: 'CEO',
			company: 'Agência Criativa Co.',
			avatar: 'FC',
			content: 'Interface moderna e intuitiva. Nossos colaboradores adoraram o app. Suporte técnico excepcional.'
		}
	];

	const faqs = [
		{
			question: 'O PontoApp está em conformidade com a Portaria 671?',
			answer: 'Sim, 100%. Nosso sistema atende todos os requisitos da Portaria 671 do MTE, incluindo registro eletrônico de ponto, assinatura digital e espelho de ponto conforme exigido pela legislação.',
			open: false
		},
		{
			question: 'Posso testar antes de contratar?',
			answer: 'Oferecemos 30 dias grátis com todas as funcionalidades liberadas e até 10 funcionários. Não pedimos cartão de crédito para o teste.',
			open: false
		},
		{
			question: 'Como funciona a geolocalização?',
			answer: 'O app mobile captura as coordenadas GPS no momento do registro. Você pode configurar cercas virtuais (geofences) para validar se o funcionário está no local de trabalho autorizado.',
			open: false
		},
		{
			question: 'Integra com sistemas de folha de pagamento?',
			answer: 'Sim, exportamos relatórios em formatos compatíveis com os principais sistemas do mercado (ADP, TOTVS, Senior, etc.) e também via API para integrações customizadas.',
			open: false
		}
	];

	let faqState = $state(faqs.map(() => false));

	function toggleFaq(index: number) {
		faqState = faqState.map((state, i) => (i === index ? !state : false));
	}
</script>

<svelte:window bind:scrollY />

<svelte:head>
	<title>PontoApp - Sistema de Controle de Ponto Eletrônico</title>
	<meta
		name="description"
		content="Simplifique o controle de ponto da sua empresa com PontoApp. Moderno, seguro e fácil de usar."
	/>
</svelte:head>

<div class="min-h-screen bg-background">
	<!-- Navbar -->
	<nav
		class="fixed top-0 z-50 w-full transition-all duration-300 {isNavScrolled
			? 'border-b border-border/40 bg-background/80 backdrop-blur-xl shadow-sm'
			: 'bg-transparent'}"
	>
		<div class="container mx-auto flex h-16 items-center justify-between px-4 lg:px-8">
			<div class="flex items-center gap-2">
				<div class="relative flex h-9 w-9 items-center justify-center rounded-lg bg-foreground">
					<span class="text-lg font-bold text-background">P</span>
					<div class="absolute -right-0.5 -top-0.5 h-2.5 w-2.5 rounded-full bg-accent-green ring-2 ring-background"></div>
				</div>
				<span class="text-xl font-bold text-foreground">PontoApp</span>
			</div>

			<div class="hidden items-center gap-8 md:flex">
				<a href="#features" class="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
					Recursos
				</a>
				<a href="#how-it-works" class="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
					Como Funciona
				</a>
				<a href="#testimonials" class="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
					Depoimentos
				</a>
				<a href="#faq" class="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
					FAQ
				</a>
			</div>

			<div class="flex items-center gap-3">
				<a
					href="/login"
					class="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
				>
					Entrar
				</a>
				<a
					href="/register"
					class="group bg-green-500 relative inline-flex h-10 items-center justify-center overflow-hidden rounded-lg bg-foreground px-5 text-sm font-medium text-background transition-all hover:bg-foreground/90"
				>
					<span class="relativz-10">Começar Grátis</span>
					<div class="absolute inset-0 -translate-x-full bg-accent-green transition-transform duration-300 group-hover:translate-x-0"></div>
					<span class="absolute inset-0 z-10 flex items-center justify-center text-accent-green-foreground opacity-0 transition-opacity duration-300 group-hover:opacity-100">Começar Grátis</span>
				</a>
			</div>
		</div>
	</nav>

	<!-- Hero Section -->
	<section id="hero" class="relative overflow-hidden pt-16">
		<!-- Background Grid -->
		<div class="pointer-events-none absolute inset-0 bg-[linear-gradient(to_right,var(--border)_1px,transparent_1px),linear-gradient(to_bottom,var(--border)_1px,transparent_1px)] bg-[size:4rem_4rem] [mask-image:radial-gradient(ellipse_60%_50%_at_50%_0%,black_70%,transparent_110%)]"></div>
		
		<!-- Green Glow -->
		<div class="pointer-events-none absolute left-1/2 top-0 h-[500px] w-[800px] -translate-x-1/2 rounded-full bg-accent-green/10 blur-[120px]"></div>

		<div class="container relative mx-auto px-4 py-24 md:py-32 lg:px-8 lg:py-40">
			<div class="mx-auto max-w-4xl text-center">
				<!-- Badge -->
				<div
					class="mb-8 bg-white border-2 border-green-300/80 inline-flex items-center gap-2 rounded-full border border-accent-green/30 bg-accent-green/10 px-4 py-2 opacity-0 {visibleSections.has('hero') ? 'animate-fade-in-up' : ''}"
					style="animation-delay: 0ms; animation-fill-mode: forwards;"
				>
					<span class="relative flex h-2 w-2">
						<span class="absolute inline-flex h-full w-full animate-ping rounded-full bg-accent-green opacity-75"></span>
						<span class="relative inline-flex h-2 w-2 rounded-full bg-accent-green"></span>
					</span>
					<span class="text-sm font-medium text-accent-green bg-white">Em conformidade com a Portaria 671</span>
				</div>

				<h1
					class="mb-6 text-4xl font-bold tracking-tight text-foreground opacity-0 md:text-5xl lg:text-6xl {visibleSections.has('hero') ? 'animate-fade-in-up' : ''}"
					style="animation-delay: 100ms; animation-fill-mode: forwards;"
				>
					Controle de Ponto
					<span class="relative">
						<span class="relative z-10 bg-gradient-to-r from-accent-green to-accent-green/70 bg-clip-text text-transparent">
							Inteligente
						</span>
						<svg class="absolute -bottom-2 left-0 w-full" viewBox="0 0 200 12" fill="none" xmlns="http://www.w3.org/2000/svg">
							<path d="M2 8.5C50 2.5 150 2.5 198 8.5" stroke="url(#green-gradient)" stroke-width="3" stroke-linecap="round"/>
							<defs>
								<linearGradient id="green-gradient" x1="0" y1="0" x2="200" y2="0">
									<stop stop-color="oklch(0.723 0.191 142.5)"/>
									<stop offset="1" stop-color="oklch(0.723 0.191 142.5 / 0.3)"/>
								</linearGradient>
							</defs>
						</svg>
					</span>
					<br />para Empresas Modernas
				</h1>

				<p
					class="mb-10 text-lg text-muted-foreground opacity-0 md:text-xl {visibleSections.has('hero') ? 'animate-fade-in-up' : ''}"
					style="animation-delay: 200ms; animation-fill-mode: forwards;"
				>
					Gerencie o registro de ponto dos seus funcionários de forma digital, segura e em
					conformidade com a legislação brasileira. Tudo em uma plataforma intuitiva.
				</p>

				<div
					class="flex flex-col items-center justify-center gap-4 opacity-0 sm:flex-row {visibleSections.has('hero') ? 'animate-fade-in-up' : ''}"
					style="animation-delay: 300ms; animation-fill-mode: forwards;"
				>
					<a
						href="/register"
						class="group bg-white inline-flex h-12 w-full items-center justify-center gap-2 rounded-xl bg-accent-green px-8 text-base font-semibold text-accent-green-foreground shadow-md shadow-accent-green/25 transition-all hover:shadow-sm hover:shadow-accent-green/30 hover:brightness-110 sm:w-auto"
					>
						Começar Gratuitamente
						<ArrowRight class="h-4 w-4 transition-transform group-hover:translate-x-1" />
					</a>
					<a
						href="#how-it-works"
						class="group border-2 border-green-300/80 inline-flex h-12 w-full items-center justify-center gap-2 rounded-xl border border-border bg-background px-8 text-base font-medium transition-all hover:border-accent-green/50 hover:bg-accent-green/5 sm:w-auto"
					>
						<Play class="h-4 w-4 text-accent-green" />
						Ver como funciona
					</a>
				</div>

				<p
					class="mt-6 flex flex-wrap items-center justify-center gap-x-6 gap-y-2 text-sm text-muted-foreground opacity-0 {visibleSections.has('hero') ? 'animate-fade-in-up' : ''}"
					style="animation-delay: 400ms; animation-fill-mode: forwards;"
				>
					<span class="inline-flex items-center gap-1.5">
						<Check class="h-4 w-4 text-accent-green" />
						Sem cartão de crédito
					</span>
					<span class="inline-flex items-center gap-1.5">
						<Check class="h-4 w-4 text-accent-green" />
						30 dias grátis
					</span>
					<span class="inline-flex items-center gap-1.5">
						<Check class="h-4 w-4 text-accent-green" />
						Cancele quando quiser
					</span>
				</p>
			</div>

			<!-- Hero Mockup -->
			<div
				class="mx-auto mt-16 max-w-5xl opacity-0 lg:mt-20 {visibleSections.has('hero') ? 'animate-fade-in-up' : ''}"
				style="animation-delay: 500ms; animation-fill-mode: forwards;"
			>
				<div class="relative">
					<!-- Glow behind mockup -->
					<div class="absolute -inset-4 rounded-3xl bg-gradient-to-r from-accent-green/20 via-transparent to-accent-green/20 blur-2xl"></div>
					
					<div class="relative overflow-hidden rounded-2xl border border-border bg-card shadow-2xl">
						<!-- Browser Chrome -->
						<div class="flex items-center gap-2 border-b border-border bg-muted/50 px-4 py-3">
							<div class="flex gap-1.5">
								<div class="h-3 w-3 rounded-full bg-destructive/60"></div>
								<div class="h-3 w-3 rounded-full bg-chart-4/60"></div>
								<div class="h-3 w-3 rounded-full bg-accent-green/60"></div>
							</div>
							<div class="ml-4 flex-1">
								<div class="mx-auto flex h-7 max-w-md items-center justify-center rounded-md bg-background/80 px-3 text-xs text-muted-foreground">
									<Shield class="mr-2 h-3 w-3 text-accent-green" />
									app.pontoapp.com.br
								</div>
							</div>
						</div>
						
						<!-- Dashboard Preview -->
						<div class="bg-gradient-to-br from-background to-muted/30 p-6 md:p-8">
							<div class="grid gap-4 md:grid-cols-4">
								{#each stats as stat, i}
									<div class="rounded-xl border border-border bg-card p-4 transition-all hover:border-accent-green/30 hover:shadow-md">
										<p class="text-2xl font-bold text-foreground md:text-3xl">{stat.value}</p>
										<p class="text-sm text-muted-foreground">{stat.label}</p>
									</div>
								{/each}
							</div>
							
							<div class="mt-6 grid gap-4 md:grid-cols-3">
								<div class="col-span-2 rounded-xl border border-border bg-card p-4">
									<div class="mb-4 flex items-center justify-between">
										<span class="font-medium text-foreground">Registros de Hoje</span>
										<span class="inline-flex items-center gap-1 text-sm text-accent-green">
											<TrendingUp class="h-4 w-4" />
											+12%
										</span>
									</div>
									<!-- Chart placeholder -->
									<div class="flex h-32 items-end gap-2">
										{#each [40, 65, 45, 80, 55, 90, 70, 85, 60, 75, 95, 80] as height, i}
											<div
												class="flex-1 rounded-t bg-gradient-to-t from-accent-green/80 to-accent-green/40 transition-all hover:from-accent-green hover:to-accent-green/60"
												style="height: {height}%"
											></div>
										{/each}
									</div>
								</div>
								<div class="rounded-xl border border-border bg-card p-4">
									<span class="font-medium text-foreground">Atividade Recente</span>
									<div class="mt-4 space-y-3">
										{#each [{ name: 'Maria S.', time: '08:02', type: 'entrada' }, { name: 'João P.', time: '08:05', type: 'entrada' }, { name: 'Ana C.', time: '12:00', type: 'intervalo' }] as activity}
											<div class="flex items-center gap-3">
												<div class="flex h-8 w-8 items-center justify-center rounded-full bg-accent-green/10 text-xs font-medium text-accent-green">
													{activity.name.split(' ').map(n => n[0]).join('')}
												</div>
												<div class="flex-1">
													<p class="text-sm font-medium text-foreground">{activity.name}</p>
													<p class="text-xs text-muted-foreground">{activity.type}</p>
												</div>
												<span class="text-xs text-muted-foreground">{activity.time}</span>
											</div>
										{/each}
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>

		<!-- Scroll indicator -->
		<div class="absolute bottom-8 left-1/2 -translate-x-1/2 animate-bounce">
			<ChevronDown class="h-6 w-6 text-muted-foreground" />
		</div>
	</section>

	<!-- Social Proof / Logos -->
	<section class="border-y border-border bg-muted/30 py-12">
		<div class="container mx-auto px-4 lg:px-8">
			<p class="mb-8 text-center text-sm font-medium text-muted-foreground">
				Empresas que confiam no PontoApp
			</p>
			<div class="flex flex-wrap items-center justify-center gap-x-12 gap-y-6 opacity-60 grayscale">
				{#each ['TechCorp', 'Logística BR', 'Construa+', 'FoodService Co', 'Varejo Express'] as company}
					<div class="flex items-center gap-2 text-lg font-semibold text-foreground">
						<Building2 class="h-5 w-5" />
						{company}
					</div>
				{/each}
			</div>
		</div>
	</section>

	<!-- Features Section -->
	<section id="features" class="py-24 lg:py-32">
		<div class="container mx-auto px-4 lg:px-8">
			<div
				class="mb-16 text-center opacity-0 {visibleSections.has('features') ? 'animate-fade-in-up' : ''}"
				style="animation-fill-mode: forwards;"
			>
				<span class="mb-4 inline-block text-sm font-semibold uppercase tracking-wider text-accent-green">
					Recursos
				</span>
				<h2 class="mb-4 text-3xl font-bold tracking-tight text-foreground md:text-4xl">
					Tudo que você precisa em um só lugar
				</h2>
				<p class="mx-auto max-w-2xl text-lg text-muted-foreground">
					Ferramentas poderosas para simplificar a gestão de ponto da sua equipe
				</p>
			</div>

			<div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
				{#each [
					{ icon: Clock, title: 'Registro Rápido', description: 'Entrada, saída e intervalos em segundos. Interface intuitiva para funcionários no app mobile.', highlight: false },
					{ icon: MapPin, title: 'Geolocalização', description: 'Registre a localização de cada marcação para garantir que funcionários estejam no local de trabalho.', highlight: true },
					{ icon: Users, title: 'Gestão Centralizada', description: 'Dashboard completo para RH e gestores acompanharem todos os registros em tempo real.', highlight: false },
					{ icon: Shield, title: 'Segurança Total', description: 'Biometria, criptografia e autenticação de dois fatores para garantir a integridade dos dados.', highlight: false },
					{ icon: TrendingUp, title: 'Relatórios Inteligentes', description: 'Espelho de ponto, horas extras, banco de horas e exportação para folha de pagamento.', highlight: true },
					{ icon: Zap, title: 'Conformidade Legal', description: '100% em conformidade com a Portaria 671 do MTE e legislação trabalhista brasileira.', highlight: false }
				] as feature, i}
					<div
						class="group relative overflow-hidden rounded-2xl border bg-card p-6 transition-all duration-300 hover:shadow-lg opacity-0 {visibleSections.has('features') ? 'animate-fade-in-up' : ''} {feature.highlight ? 'border-accent-green/30 hover:border-accent-green/50' : 'border-border hover:border-accent-green/30'}"
						style="animation-delay: {i * 100}ms; animation-fill-mode: forwards;"
					>
						{#if feature.highlight}
							<div class="absolute -right-12 -top-12 h-24 w-24 rounded-full bg-accent-green/10 blur-2xl transition-all group-hover:bg-accent-green/20"></div>
						{/if}
						
						<div class="relative">
							<div class="mb-4 inline-flex h-12 w-12 items-center justify-center rounded-xl {feature.highlight ? 'bg-accent-green/10' : 'bg-muted'} transition-colors group-hover:bg-accent-green/10">
								<svelte:component this={feature.icon} class="h-6 w-6 {feature.highlight ? 'text-accent-green' : 'text-foreground'} transition-colors group-hover:text-accent-green" />
							</div>
							<h3 class="mb-2 text-xl font-semibold text-foreground">{feature.title}</h3>
							<p class="text-muted-foreground">{feature.description}</p>
						</div>
					</div>
				{/each}
			</div>
		</div>
	</section>

	<!-- How It Works -->
	<section id="how-it-works" class="border-t border-border bg-muted/30 py-24 lg:py-32">
		<div class="container mx-auto px-4 lg:px-8">
			<div
				class="mb-16 text-center opacity-0 {visibleSections.has('how-it-works') ? 'animate-fade-in-up' : ''}"
				style="animation-fill-mode: forwards;"
			>
				<span class="mb-4 inline-block text-sm font-semibold uppercase tracking-wider text-accent-green">
					Simples
				</span>
				<h2 class="mb-4 text-3xl font-bold tracking-tight text-foreground md:text-4xl">
					Comece em 3 passos
				</h2>
				<p class="text-lg text-muted-foreground">
					Configure e comece a usar em minutos
				</p>
			</div>

			<div class="relative">
				<!-- Connection Line -->
				<div class="absolute left-1/2 top-8 hidden h-0.5 w-[60%] -translate-x-1/2 bg-gradient-to-r from-transparent via-accent-green/30 to-transparent lg:block"></div>

				<div class="grid gap-8 md:grid-cols-3">
					{#each [
						{ step: '01', title: 'Cadastre sua Empresa', description: 'Crie sua conta gratuitamente em menos de 2 minutos. Sem burocracia.' },
						{ step: '02', title: 'Adicione Funcionários', description: 'Cadastre seus colaboradores pelo painel web e envie convites para o app.' },
						{ step: '03', title: 'Comece a Usar', description: 'Funcionários registram ponto pelo app e você acompanha tudo em tempo real.' }
					] as item, i}
						<div
							class="relative text-center opacity-0 {visibleSections.has('how-it-works') ? 'animate-fade-in-up' : ''}"
							style="animation-delay: {i * 150}ms; animation-fill-mode: forwards;"
						>
							<div class="relative mx-auto mb-6 flex h-16 w-16 items-center justify-center">
								<div class="absolute inset-0 rounded-2xl bg-accent-green/10"></div>
								<div class="absolute inset-1 rounded-xl bg-background"></div>
								<span class="relative text-2xl font-bold text-accent-green">{item.step}</span>
							</div>
							<h3 class="mb-2 text-xl font-semibold text-foreground">{item.title}</h3>
							<p class="text-muted-foreground">{item.description}</p>
						</div>
					{/each}
				</div>
			</div>
		</div>
	</section>

	<!-- Testimonials -->
	<section id="testimonials" class="py-24 lg:py-32">
		<div class="container mx-auto px-4 lg:px-8">
			<div
				class="mb-16 text-center opacity-0 {visibleSections.has('testimonials') ? 'animate-fade-in-up' : ''}"
				style="animation-fill-mode: forwards;"
			>
				<span class="mb-4 inline-block text-sm font-semibold uppercase tracking-wider text-accent-green">
					Depoimentos
				</span>
				<h2 class="mb-4 text-3xl font-bold tracking-tight text-foreground md:text-4xl">
					O que nossos clientes dizem
				</h2>
				<p class="text-lg text-muted-foreground">
					Histórias reais de empresas que transformaram sua gestão de ponto
				</p>
			</div>

			<div class="grid gap-6 md:grid-cols-3">
				{#each testimonials as testimonial, i}
					<div
						class="relative rounded-2xl border border-border bg-card p-6 transition-all hover:border-accent-green/30 hover:shadow-lg opacity-0 {visibleSections.has('testimonials') ? 'animate-fade-in-up' : ''}"
						style="animation-delay: {i * 100}ms; animation-fill-mode: forwards;"
					>
						<div class="mb-4 flex gap-1">
							{#each Array(5) as _}
								<Star class="h-4 w-4 fill-accent-green text-accent-green" />
							{/each}
						</div>
						<p class="mb-6 text-foreground">"{testimonial.content}"</p>
						<div class="flex items-center gap-3">
							<div class="flex h-10 w-10 items-center justify-center rounded-full bg-accent-green/10 text-sm font-semibold text-accent-green">
								{testimonial.avatar}
							</div>
							<div>
								<p class="font-medium text-foreground">{testimonial.name}</p>
								<p class="text-sm text-muted-foreground">{testimonial.role}, {testimonial.company}</p>
							</div>
						</div>
					</div>
				{/each}
			</div>
		</div>
	</section>

	<!-- FAQ -->
	<section id="faq" class="border-t border-border bg-muted/30 py-24 lg:py-32">
		<div class="container mx-auto px-4 lg:px-8">
			<div
				class="mb-16 text-center opacity-0 {visibleSections.has('faq') ? 'animate-fade-in-up' : ''}"
				style="animation-fill-mode: forwards;"
			>
				<span class="mb-4 inline-block text-sm font-semibold uppercase tracking-wider text-accent-green">
					FAQ
				</span>
				<h2 class="mb-4 text-3xl font-bold tracking-tight text-foreground md:text-4xl">
					Perguntas Frequentes
				</h2>
				<p class="text-lg text-muted-foreground">
					Tire suas dúvidas sobre o PontoApp
				</p>
			</div>

			<div class="mx-auto max-w-3xl space-y-4">
				{#each faqs as faq, i}
					<div
						class="overflow-hidden rounded-xl border border-border bg-card transition-all hover:border-accent-green/30 opacity-0 {visibleSections.has('faq') ? 'animate-fade-in-up' : ''}"
						style="animation-delay: {i * 100}ms; animation-fill-mode: forwards;"
					>
						<button
							class="flex w-full items-center justify-between p-6 text-left"
							onclick={() => toggleFaq(i)}
						>
							<span class="font-medium text-foreground">{faq.question}</span>
							<ChevronDown class="h-5 w-5 text-muted-foreground transition-transform {faqState[i] ? 'rotate-180' : ''}" />
						</button>
						<div class="grid transition-all duration-300 {faqState[i] ? 'grid-rows-[1fr]' : 'grid-rows-[0fr]'}">
							<div class="overflow-hidden">
								<p class="px-6 pb-6 text-muted-foreground">{faq.answer}</p>
							</div>
						</div>
					</div>
				{/each}
			</div>
		</div>
	</section>

	<!-- CTA -->
	<section class="py-24 lg:py-32">
		<div class="container mx-auto px-4 lg:px-8">
			<div class="relative overflow-hidden rounded-3xl bg-foreground p-8 md:p-12 lg:p-16">
				<!-- Background decoration -->
				<div class="absolute -right-24 -top-24 h-64 w-64 rounded-full bg-accent-green/20 blur-3xl"></div>
				<div class="absolute -bottom-24 -left-24 h-64 w-64 rounded-full bg-accent-green/10 blur-3xl"></div>
				
				<div class="relative mx-auto max-w-3xl text-center">
					<h2 class="mb-4 text-3xl font-bold tracking-tight text-background md:text-4xl lg:text-5xl">
						Pronto para modernizar seu controle de ponto?
					</h2>
					<p class="mb-8 text-lg text-background/70">
						Junte-se a mais de 500 empresas que já simplificaram sua gestão de ponto com o PontoApp.
					</p>

					<div class="mb-8 flex flex-wrap items-center justify-center gap-6">
						<div class="flex items-center gap-2 text-background/80">
							<Check class="h-5 w-5 text-accent-green" />
							<span>Até 10 funcionários grátis</span>
						</div>
						<div class="flex items-center gap-2 text-background/80">
							<Check class="h-5 w-5 text-accent-green" />
							<span>Suporte técnico incluído</span>
						</div>
						<div class="flex items-center gap-2 text-background/80">
							<Check class="h-5 w-5 text-accent-green" />
							<span>Sem cartão de crédito</span>
						</div>
					</div>
	
					<a
						href="/register"
						class="group inline-flex h-14 items-center justify-center gap-2 rounded-xl bg-accent-green px-10 text-lg font-semibold text-accent-green-foreground shadow-lg shadow-accent-green/25 transition-all hover:shadow-xl hover:shadow-accent-green/40 hover:brightness-110"
					>
						Começar Agora Gratuitamente
						<ArrowRight class="h-5 w-5 transition-transform group-hover:translate-x-1" />
					</a>
				</div>
			</div>
		</div>
	</section>

	<!-- Footer -->
	<footer class="border-t border-border bg-background py-12 lg:py-16">
		<div class="container mx-auto px-4 lg:px-8">
			<div class="grid gap-8 md:grid-cols-2 lg:grid-cols-5">
				<!-- Brand -->
				<div class="lg:col-span-2">
					<div class="mb-4 flex items-center gap-2">
						<div class="relative flex h-9 w-9 items-center justify-center rounded-lg bg-foreground">
							<span class="text-lg font-bold text-background">P</span>
							<div class="absolute -right-0.5 -top-0.5 h-2.5 w-2.5 rounded-full bg-accent-green ring-2 ring-background"></div>
						</div>
						<span class="text-xl font-bold text-foreground">PontoApp</span>
					</div>
					<p class="mb-6 max-w-sm text-sm text-muted-foreground">
						Controle de ponto eletrônico moderno, seguro e em conformidade com a legislação brasileira.
					</p>
					<div class="flex items-center gap-2 rounded-lg border border-accent-green/30 bg-accent-green/10 px-3 py-2 text-sm text-accent-green">
						<Shield class="h-4 w-4" />
						<span>Certificado Portaria 671/MTE</span>
					</div>
				</div>

				<!-- Product -->
				<div>
					<h3 class="mb-4 text-sm font-semibold text-foreground">Produto</h3>
					<ul class="space-y-3 text-sm text-muted-foreground">
						<li><a href="#features" class="transition-colors hover:text-accent-green">Funcionalidades</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Preços</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Mobile App</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">API</a></li>
					</ul>
				</div>

				<!-- Company -->
				<div>
					<h3 class="mb-4 text-sm font-semibold text-foreground">Empresa</h3>
					<ul class="space-y-3 text-sm text-muted-foreground">
						<li><a href="#" class="transition-colors hover:text-accent-green">Sobre</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Blog</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Carreiras</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Contato</a></li>
					</ul>
				</div>

				<!-- Legal -->
				<div>
					<h3 class="mb-4 text-sm font-semibold text-foreground">Legal</h3>
					<ul class="space-y-3 text-sm text-muted-foreground">
						<li><a href="#" class="transition-colors hover:text-accent-green">Termos de Uso</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Privacidade</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">LGPD</a></li>
						<li><a href="#" class="transition-colors hover:text-accent-green">Cookies</a></li>
					</ul>
				</div>
			</div>

			<div class="mt-12 flex flex-col items-center justify-between gap-4 border-t border-border pt-8 md:flex-row">
				<p class="text-sm text-muted-foreground">
					© 2025 PontoApp. Todos os direitos reservados.
				</p>
				<div class="flex items-center gap-6">
					<a href="#" class="text-muted-foreground transition-colors hover:text-accent-green">
						<svg class="h-5 w-5" fill="currentColor" viewBox="0 0 24 24"><path d="M24 4.557c-.883.392-1.832.656-2.828.775 1.017-.609 1.798-1.574 2.165-2.724-.951.564-2.005.974-3.127 1.195-.897-.957-2.178-1.555-3.594-1.555-3.179 0-5.515 2.966-4.797 6.045-4.091-.205-7.719-2.165-10.148-5.144-1.29 2.213-.669 5.108 1.523 6.574-.806-.026-1.566-.247-2.229-.616-.054 2.281 1.581 4.415 3.949 4.89-.693.188-1.452.232-2.224.084.626 1.956 2.444 3.379 4.6 3.419-2.07 1.623-4.678 2.348-7.29 2.04 2.179 1.397 4.768 2.212 7.548 2.212 9.142 0 14.307-7.721 13.995-14.646.962-.695 1.797-1.562 2.457-2.549z"/></svg>
					</a>
					<a href="#" class="text-muted-foreground transition-colors hover:text-accent-green">
						<svg class="h-5 w-5" fill="currentColor" viewBox="0 0 24 24"><path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/></svg>
					</a>
					<a href="#" class="text-muted-foreground transition-colors hover:text-accent-green">
						<svg class="h-5 w-5" fill="currentColor" viewBox="0 0 24 24"><path d="M20.447 20.452h-3.554v-5.569c0-1.328-.027-3.037-1.852-3.037-1.853 0-2.136 1.445-2.136 2.939v5.667H9.351V9h3.414v1.561h.046c.477-.9 1.637-1.85 3.37-1.85 3.601 0 4.267 2.37 4.267 5.455v6.286zM5.337 7.433c-1.144 0-2.063-.926-2.063-2.065 0-1.138.92-2.063 2.063-2.063 1.14 0 2.064.925 2.064 2.063 0 1.139-.925 2.065-2.064 2.065zm1.782 13.019H3.555V9h3.564v11.452zM22.225 0H1.771C.792 0 0 .774 0 1.729v20.542C0 23.227.792 24 1.771 24h20.451C23.2 24 24 23.227 24 22.271V1.729C24 .774 23.2 0 22.222 0h.003z"/></svg>
					</a>
				</div>
			</div>
		</div>
	</footer>
</div>

<style>
	@keyframes fade-in-up {
		from {
			opacity: 0;
			transform: translateY(20px);
		}
		to {
			opacity: 1;
			transform: translateY(0);
		}
	}

	:global(.animate-fade-in-up) {
		animation: fade-in-up 0.6s ease-out;
	}
</style>