<script lang="ts">
  let {
    variant = 'default',
    size = 'default',
    disabled = false,
    type = 'button',
    onclick,
    class: className = '',
    children
  }: {
    variant?: 'default' | 'outline' | 'ghost' | 'destructive';
    size?: 'default' | 'sm' | 'lg' | 'icon';
    disabled?: boolean;
    type?: 'button' | 'submit' | 'reset';
    onclick?: (e: MouseEvent) => void;
    class?: string;
    children: any;
  } = $props();
</script>

<button
  {type}
  {disabled}
  {onclick}
  class={`btn btn-${variant} btn-${size} ${className}`}
  class:disabled
>
  {@render children()}
</button>

<style>
  .btn {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    font-family: var(--font-sans);
    font-weight: 600;
    letter-spacing: -0.01em;
    border-radius: var(--radius-md);
    transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    cursor: pointer;
    border: none;
    white-space: nowrap;
    position: relative;
    overflow: hidden;
  }

  .btn::before {
    content: '';
    position: absolute;
    inset: 0;
    background: linear-gradient(
      135deg,
      transparent 0%,
      color-mix(in oklch, currentColor, transparent 90%) 50%,
      transparent 100%
    );
    opacity: 0;
    transition: opacity 0.3s ease;
  }

  .btn:hover::before {
    opacity: 1;
  }

  /* Sizes */
  .btn-default {
    padding: 0.625rem 1.25rem;
    font-size: 0.875rem;
    height: 2.5rem;
  }

  .btn-sm {
    padding: 0.5rem 1rem;
    font-size: 0.813rem;
    height: 2rem;
  }

  .btn-lg {
    padding: 0.75rem 1.5rem;
    font-size: 1rem;
    height: 3rem;
  }

  .btn-icon {
    padding: 0.5rem;
    width: 2.5rem;
    height: 2.5rem;
  }

  /* Variants */
  .btn-default {
    background: var(--primary);
    color: var(--primary-foreground);
    box-shadow: 
      0 1px 3px 0 color-mix(in oklch, var(--primary), transparent 50%),
      0 0 0 1px color-mix(in oklch, var(--primary), transparent 80%);
  }

  .btn-default:hover:not(.disabled) {
    background: color-mix(in oklch, var(--primary), black 10%);
    box-shadow: 
      0 4px 6px -1px color-mix(in oklch, var(--primary), transparent 40%),
      0 2px 4px -2px color-mix(in oklch, var(--primary), transparent 40%);
    transform: translateY(-1px);
  }

  .btn-default:active:not(.disabled) {
    transform: translateY(0);
    box-shadow: 
      0 1px 2px 0 color-mix(in oklch, var(--primary), transparent 50%);
  }

  .btn-outline {
    background: transparent;
    color: var(--foreground);
    border: 1px solid var(--border);
    box-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.05);
  }

  .btn-outline:hover:not(.disabled) {
    background: var(--muted);
    border-color: var(--primary);
  }

  .btn-ghost {
    background: transparent;
    color: var(--foreground);
  }

  .btn-ghost:hover:not(.disabled) {
    background: var(--muted);
  }

  .btn-destructive {
    background: var(--destructive);
    color: white;
    box-shadow: 
      0 1px 3px 0 color-mix(in oklch, var(--destructive), transparent 50%),
      0 0 0 1px color-mix(in oklch, var(--destructive), transparent 80%);
  }

  .btn-destructive:hover:not(.disabled) {
    background: color-mix(in oklch, var(--destructive), black 10%);
    transform: translateY(-1px);
  }

  .btn.disabled {
    opacity: 0.5;
    cursor: not-allowed;
    pointer-events: none;
  }

  .btn:focus-visible {
    outline: none;
    box-shadow: 0 0 0 3px color-mix(in oklch, var(--primary), transparent 80%);
  }
</style>