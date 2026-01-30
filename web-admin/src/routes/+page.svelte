<script lang="ts">
	import CTA from '$lib/components/features/landing/CTA.svelte';
	import FAQ from '$lib/components/features/landing/FAQ.svelte';
	import Features from '$lib/components/features/landing/Features.svelte';
	import Hero from '$lib/components/features/landing/Hero.svelte';
	import LogoMarquee from '$lib/components/features/landing/LogoMarquee.svelte';
	import Navbar from '$lib/components/features/landing/Navbar.svelte';
	import Testimonials from '$lib/components/features/landing/Testimonials.svelte';
	import { onMount } from 'svelte';

	// 1. Estado reativo para animações de scroll
	let scrollY = $state(0);
	let isNavScrolled = $derived(scrollY > 50);
	let visibleSections = $state<Set<string>>(new Set());

	// 2. Dados que serão passados para os componentes
	const stats = [
		{ value: '50mil+', label: 'Registros por mês' },
		{ value: '99.9%', label: 'Uptime garantido' },
		{ value: '500+', label: 'Empresas ativas' },
		{ value: '<2s', label: 'Tempo de registro' }
	];

	const faqs = [
		{
			question: 'O PontoApp está de acordo com a Portaria 671?',
			answer: 'Sim, nossa plataforma foi desenvolvida estritamente seguindo as normas do Ministério do Trabalho e Emprego.'
		},
		{
			question: 'Como funciona o registro offline?',
			answer: 'O colaborador pode registrar o ponto mesmo sem internet. Os dados são sincronizados assim que uma conexão for detectada.'
		}
	];

	onMount(() => {
		const observer = new IntersectionObserver(
			(entries) => {
				entries.forEach((entry) => {
					if (entry.isIntersecting) {
						// No Svelte 5, atualizamos o Set criando uma nova instância para disparar a reatividade
						visibleSections.add(entry.target.id);
						visibleSections = new Set(visibleSections);
					}
				});
			},
			{ threshold: 0.1 }
		);

		document.querySelectorAll('section[id]').forEach((s) => observer.observe(s));
		return () => observer.disconnect();
	});
</script>

<svelte:window bind:scrollY />

<Navbar {isNavScrolled} />

<main>
  <Hero {visibleSections} {stats} />
  <LogoMarquee />
  <Features {visibleSections} />
  <Testimonials />
  <FAQ {faqs} />
  <CTA />
</main>

<footer class="py-12 border-t border-border/50 text-center text-muted-foreground bg-muted/20">
	<p>&copy; 2024 PontoApp. Todos os direitos reservados.</p>
</footer>