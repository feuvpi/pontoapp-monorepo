<script lang="ts">
  const logos = ['Google', 'Microsoft', 'Amazon', 'Meta', 'Netflix', 'Airbnb'];
</script>

<section class="py-12 border-y border-border/50 bg-muted/30 overflow-hidden">
  <div class="container mx-auto px-4 mb-8 text-center">
    <p class="text-sm font-medium text-muted-foreground uppercase tracking-widest">Empresas que confiam em nós</p>
  </div>
  
  <div class="relative flex overflow-x-hidden">
    <div class="animate-marquee flex items-center gap-16 pr-16">
      {#each [...logos, ...logos] as logo} <span class="text-2xl font-bold text-muted-foreground/40 grayscale hover:grayscale-0 transition-all cursor-default">
          {logo}
        </span>
      {/each}
    </div>
  </div>
</section>