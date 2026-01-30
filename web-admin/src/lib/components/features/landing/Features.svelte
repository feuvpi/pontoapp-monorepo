<script lang="ts">
  import { Shield, Zap, TrendingUp } from 'lucide-svelte';
  
  interface Feature {
    title: string;
    desc: string;
    icon: any;
  }
  
  const features: Feature[] = [
    { title: 'Segurança Total', desc: 'Proteção de dados bancários.', icon: Shield },
    { title: 'Alta Performance', desc: 'Carregamento em milissegundos.', icon: Zap },
    { title: 'Analytics Real-time', desc: 'Veja tudo em tempo real.', icon: TrendingUp },
  ];

  let { visibleSections } = $props(); // Recebe o estado do pai
</script>

{#snippet featureCard({ title, desc, icon: Icon }: Feature)}
  <div class="glass-card p-8 rounded-2xl flex flex-col gap-4">
    <div class="w-12 h-12 rounded-lg bg-primary/10 flex items-center justify-center text-primary">
      <Icon size={24} />
    </div>
    <h3 class="text-xl font-bold">{title}</h3>
    <p class="text-muted-foreground">{desc}</p>
  </div>
{/snippet}

<section id="features" class="py-24 container mx-auto px-4">
  <div class="grid md:grid-cols-3 gap-8">
    {#each features as item}
      {@render featureCard(item)}
    {/each}
  </div>
</section>