<script lang="ts">
  import { ArrowRight, Play, Shield, Check } from 'lucide-svelte';
  let { visibleSections, stats } = $props();
</script>

<section id="hero" class="relative overflow-hidden pt-32 pb-20">
  <div class="absolute top-0 left-1/2 -translate-x-1/2 w-full h-full -z-10 opacity-30">
    <div class="absolute top-[-10%] left-[-10%] w-[40%] h-[40%] rounded-full bg-primary/20 blur-[120px]"></div>
    <div class="absolute bottom-[10%] right-[-5%] w-[30%] h-[30%] rounded-full bg-primary/10 blur-[100px]"></div>
  </div>

  <div class="container mx-auto px-4 text-center">
    <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 border border-primary/20 text-primary text-xs font-bold uppercase tracking-wider mb-8">
      <span class="relative flex h-2 w-2">
        <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-primary opacity-75"></span>
        <span class="relative inline-flex h-2 w-2 rounded-full bg-primary"></span>
      </span>
      Portaria 671 MTE
    </div>

    <h1 class="text-5xl md:text-7xl font-extrabold font-display leading-[1.1] mb-6 tracking-tight">
      Controle de ponto <br/>
      <span class="bg-gradient-to-r from-green-500 to-primary/80 bg-clip-text text-transparent">inteligente</span> para times.
    </h1>

    <p class="max-w-2xl mx-auto text-lg md:text-xl text-muted-foreground mb-10 leading-relaxed">
      Gerencie o registro de ponto dos seus funcionários de forma digital, segura e em conformidade com a legislação brasileira.
    </p>

    <div class="flex flex-col sm:flex-row items-center justify-center gap-4">
      <button class="w-full sm:w-auto px-8 py-4 bg-primary text-primary-foreground rounded-xl font-bold text-lg shadow-xl shadow-primary/20 hover:scale-[1.02] transition-all flex items-center justify-center gap-2">
        Começar Teste Grátis <ArrowRight size={20} />
      </button>
      <button class="w-full border-2 border-green-300/80 sm:w-auto px-8 py-4 glass-card rounded-xl font-bold text-lg flex items-center justify-center gap-2">
        <Play size={20} class="fill-current" /> Ver Demo
      </button>
    </div>

    <div class="mt-20 relative max-w-5xl mx-auto">
      <div class="absolute -inset-1 bg-gradient-to-r from-primary/20 to-transparent blur-2xl -z-10"></div>
      <div class="glass-card rounded-2xl overflow-hidden border-white/10 shadow-2xl">
        <div class="h-10 bg-muted/50 border-b border-white/5 flex items-center px-4 gap-2">
           <div class="flex gap-1.5"><div class="w-3 h-3 rounded-full bg-red-500/50"></div><div class="w-3 h-3 rounded-full bg-yellow-500/50"></div><div class="w-3 h-3 rounded-full bg-green-500/50"></div></div>
        </div>
        <div class="p-8 grid grid-cols-1 md:grid-cols-4 gap-4 bg-background/40">
           {#each stats as stat}
             <div class="p-6 rounded-xl bg-white/5 border border-white/10">
                <div class="text-3xl font-bold font-display">{stat.value}</div>
                <div class="text-sm text-muted-foreground uppercase tracking-tighter">{stat.label}</div>
             </div>
           {/each}
        </div>
      </div>
    </div>
  </div>
</section>