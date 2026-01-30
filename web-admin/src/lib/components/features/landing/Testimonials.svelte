<script lang="ts">
  import { Star } from 'lucide-svelte';

  const testimonials = [
    {
      name: 'Carolina Mendes',
      role: 'Gerente de RH',
      company: 'TechStart Brasil',
      avatar: 'CM',
      content: 'Reduzimos em 80% o tempo gasto com fechamento de folha. A interface é intuitiva e o suporte é excepcional.'
    },
    {
      name: 'Ricardo Alves',
      role: 'Diretor de Operações',
      company: 'Logística Express',
      avatar: 'RA',
      content: 'O registro offline salvou nossa operação em campo. Nossos motoristas batem ponto sem complicações.'
    },
    {
      name: 'Juliana Silva',
      role: 'CEO',
      company: 'Design Hub',
      avatar: 'JS',
      content: 'Uma ferramenta moderna que reflete a cultura da nossa empresa. Praticidade e segurança jurídica total.'
    }
  ];
</script>

<section id="depoimentos" class="py-24 bg-background">
  <div class="container mx-auto px-4">
    <div class="text-center mb-16">
      <h2 class="text-3xl md:text-4xl font-bold font-display mb-4">Quem usa, aprova</h2>
      <p class="text-muted-foreground">Junte-se a centenas de empresas que modernizaram sua gestão.</p>
    </div>

    <div class="grid md:grid-cols-3 gap-8">
      {#each testimonials as t}
        <div class="glass-card p-8 rounded-2xl flex flex-col gap-6">
          <div class="flex gap-1 text-primary">
            {#each Array(5) as _}
              <Star size={16} fill="currentColor" />
            {/each}
          </div>
          <p class="text-lg italic text-foreground/90">"{t.content}"</p>
          <div class="flex items-center gap-4 mt-auto">
            <div class="w-12 h-12 rounded-full bg-primary/20 flex items-center justify-center font-bold text-primary font-display">
              {t.avatar}
            </div>
            <div>
              <div class="font-bold font-display leading-tight">{t.name}</div>
              <div class="text-sm text-muted-foreground">{t.role} • {t.company}</div>
            </div>
          </div>
        </div>
      {/each}
    </div>
  </div>
</section>