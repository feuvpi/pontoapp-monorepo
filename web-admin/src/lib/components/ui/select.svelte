<script lang="ts">
  import { clickOutside } from '$lib/utils/click-outside';

  interface Option {
    value: string | number;
    label: string;
    subtitle?: string;
  }

  let {
    options,
    value = $bindable(''),
    placeholder = 'Selecione...',
    label,
    class: className = ''
  }: {
    options: Option[];
    value?: string | number;
    placeholder?: string;
    label?: string;
    class?: string;
  } = $props();

  let isOpen = $state(false);
  
  const selectedOption = $derived(
    options.find(opt => opt.value === value)
  );

  function selectOption(optionValue: string | number) {
    value = optionValue;
    isOpen = false;
  }
</script>

<div class={`select-wrapper ${className}`}>
  {#if label}
    <label class="select-label">{label}</label>
  {/if}
  
  <div class="select-container">
    <button
      class="select-trigger"
      class:open={isOpen}
      onclick={() => isOpen = !isOpen}
      type="button"
    >
      <span class="select-value">
        {selectedOption?.label || placeholder}
      </span>
      <svg
        class="select-arrow"
        class:rotate={isOpen}
        width="16"
        height="16"
        viewBox="0 0 16 16"
        fill="none"
      >
        <path
          d="M4 6L8 10L12 6"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </button>

    {#if isOpen}
      <div 
        class="select-dropdown"
        use:clickOutside={() => isOpen = false}
      >
        {#each options as option}
          <button
            class="select-option"
            class:selected={option.value === value}
            onclick={() => selectOption(option.value)}
            type="button"
          >
            <div>
              <div class="option-label">{option.label}</div>
              {#if option.subtitle}
                <div class="option-subtitle">{option.subtitle}</div>
              {/if}
            </div>
            {#if option.value === value}
              <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
                <path
                  d="M13 4L6 11L3 8"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
              </svg>
            {/if}
          </button>
        {/each}
      </div>
    {/if}
  </div>
</div>

<style>
  .select-wrapper {
    width: 100%;
  }

  .select-label {
    display: block;
    font-size: 0.875rem;
    font-weight: 500;
    color: var(--foreground);
    margin-bottom: 0.5rem;
    letter-spacing: -0.01em;
  }

  .select-container {
    position: relative;
  }

  .select-trigger {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.75rem 1rem;
    background: var(--background);
    border: 1px solid var(--border);
    border-radius: var(--radius-md);
    cursor: pointer;
    transition: all 0.2s ease;
    font-size: 0.875rem;
  }

  .select-trigger:hover {
    border-color: var(--primary);
  }

  .select-trigger.open {
    border-color: var(--primary);
    box-shadow: 0 0 0 3px color-mix(in oklch, var(--primary), transparent 90%);
  }

  .select-value {
    color: var(--foreground);
  }

  .select-arrow {
    color: var(--muted-foreground);
    transition: transform 0.2s ease;
  }

  .select-arrow.rotate {
    transform: rotate(180deg);
  }

  .select-dropdown {
    position: absolute;
    top: calc(100% + 0.5rem);
    left: 0;
    right: 0;
    background: var(--background);
    border: 1px solid var(--border);
    border-radius: var(--radius-lg);
    box-shadow: 
      0 10px 15px -3px rgb(0 0 0 / 0.1),
      0 4px 6px -4px rgb(0 0 0 / 0.1);
    z-index: 50;
    max-height: 300px;
    overflow-y: auto;
    animation: dropdown-in 0.15s ease-out;
  }

  @keyframes dropdown-in {
    from {
      opacity: 0;
      transform: translateY(-4px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  .select-option {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.75rem 1rem;
    background: transparent;
    border: none;
    cursor: pointer;
    transition: background 0.15s ease;
    text-align: left;
  }

  .select-option:hover {
    background: var(--muted);
  }

  .select-option.selected {
    background: color-mix(in oklch, var(--primary), transparent 90%);
  }

  .option-label {
    font-size: 0.875rem;
    color: var(--foreground);
  }

  .option-subtitle {
    font-size: 0.75rem;
    color: var(--muted-foreground);
    margin-top: 0.125rem;
  }
</style>