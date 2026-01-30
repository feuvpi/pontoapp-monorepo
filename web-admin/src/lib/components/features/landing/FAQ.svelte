<script lang="ts">
  import { ChevronDown } from 'lucide-svelte';
  
  interface FAQ {
    question: string;
    answer: string;
  }
  
  let { faqs } = $props<{ faqs: FAQ[] }>();
  let openIndex = $state<number | null>(null);

  function toggle(i: number) {
    openIndex = openIndex === i ? null : i;
  }
</script>

{#snippet faqItem(faq: FAQ, i: number)}
  <div class="glass-card rounded-2xl overflow-hidden mb-4 transition-all">
    <button 
      onclick={() => toggle(i)}
      class="w-full p-6 text-left flex justify-between items-center group"
    >
      <span class="text-lg font-bold font-display transition-colors group-hover:text-primary">
        {faq.question}
      </span>
      <ChevronDown 
        class="transition-transform duration-300 {openIndex === i ? 'rotate-180 text-primary' : 'text-muted-foreground'}" 
      />
    </button>
    
    <div class="grid transition-all duration-300 ease-in-out {openIndex === i ? 'grid-rows-[1fr] opacity-100' : 'grid-rows-[0fr] opacity-0'}">
      <div class="overflow-hidden">
        <p class="px-6 pb-6 text-muted-foreground leading-relaxed">
          {faq.answer}
        </p>
      </div>
    </div>
  </div>
{/snippet}

<section id="faq" class="py-24 bg-muted/30">
  <div class="container mx-auto px-4 max-w-3xl">
    <div class="text-center mb-16">
      <h2 class="text-3xl md:text-4xl font-bold font-display mb-4">Dúvidas Frequentes</h2>
      <p class="text-muted-foreground">Tudo o que você precisa saber para começar hoje mesmo.</p>
    </div>

    <div class="space-y-4">
      {#each faqs as item, i}
        {@render faqItem(item, i)}
      {/each}
    </div>
  </div>
</section>