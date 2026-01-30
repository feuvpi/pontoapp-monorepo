<script lang="ts">
  let { isNavScrolled = false } = $props();
</script>

<nav
  class="fixed top-0 z-50 w-full transition-all duration-300 {isNavScrolled
    ? 'border-b border-white/10 bg-background/60 backdrop-blur-xl shadow-sm'
    : 'bg-transparent'}"
>
  <div class="container mx-auto flex h-16 items-center justify-between px-4 lg:px-8">
    <div class="flex items-center gap-2">
      <div class="relative flex h-9 w-9 items-center justify-center rounded-lg bg-foreground">
        <span class="text-lg font-bold text-background">P</span>
        <div class="absolute -right-0.5 -top-0.5 h-2.5 w-2.5 rounded-full bg-primary ring-2 ring-background"></div>
      </div>
      <span class="text-xl font-bold font-display tracking-tight text-foreground">PontoApp</span>
    </div>

    <div class="hidden items-center gap-8 md:flex">
      {#each ['Recursos', 'Como Funciona', 'Depoimentos', 'FAQ'] as link}
        <a href="#{link.toLowerCase().replace(' ', '-')}" 
           class="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground">
          {link}
        </a>
      {/each}
    </div>

    <div class="flex items-center gap-3">
      <a href="/login" class="text-sm font-medium text-muted-foreground hover:text-foreground px-4">Entrar</a>
      <a href="/register" class=" bg-green-500/90 text-primary-foreground px-5 py-2 rounded-lg text-sm font-semibold hover:brightness-110 transition-all shadow-lg shadow-primary/20">
        Começar Grátis
      </a>
    </div>
  </div>
</nav>