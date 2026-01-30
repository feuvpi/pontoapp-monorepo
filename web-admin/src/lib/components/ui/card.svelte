<script lang="ts">
  let {
    variant = 'default',
    class: className = '',
    children
  }: {
    variant?: 'default' | 'glass';
    class?: string;
    children: any;
  } = $props();
</script>

<div class={`card ${variant} ${className}`}>
  {@render children()}
</div>

<style>
  .card {
    background: var(--card);
    border: 1px solid var(--border);
    border-radius: var(--radius-xl);
    padding: 1.5rem;
    transition: all 0.3s ease;
  }

  .card.glass {
    background: color-mix(in oklch, var(--background), transparent 20%);
    backdrop-filter: blur(12px);
    border: 1px solid color-mix(in oklch, var(--foreground), transparent 90%);
  }

  .card.glass:hover {
    border-color: color-mix(in oklch, var(--primary), transparent 70%);
  }
</style>