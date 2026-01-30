<script lang="ts">
  import { ArrowRight } from 'lucide-svelte';
</script>

<section class="py-20">
  <div class="container mx-auto px-4">
    <div class="relative overflow-hidden rounded-3xl bg-primary px-8 py-16 md:px-16 text-center shadow-2xl shadow-primary/30">
      <div class="absolute top-0 right-0 -translate-y-1/2 translate-x-1/2 w-96 h-96 bg-white/10 rounded-full blur-3xl"></div>
      
      <div class="relative z-10 max-w-3xl mx-auto">
        <h2 class="text-3xl md:text-5xl font-bold font-display text-primary-foreground mb-6">
          Pronto para modernizar seu RH?
        </h2>
        <p class="text-primary-foreground/80 text-lg mb-10 leading-relaxed">
          Comece agora seu teste grátis de 14 dias. Sem cartão de crédito, sem burocracia.
        </p>
        <div class="flex flex-col sm:row items-center justify-center gap-4">
          <button class="bg-white text-primary px-8 py-4 rounded-xl font-bold text-lg hover:bg-white/90 transition-colors flex items-center gap-2">
            Criar Conta Grátis <ArrowRight size={20} />
          </button>
          <p class="text-primary-foreground/60 text-sm">
            Dúvidas? <a href="mailto:contato@pontoapp.com" class="underline hover:text-white">Fale com um especialista</a>
          </p>
        </div>
      </div>
    </div>
  </div>
</section>