<script lang="ts">
  let {
    type = 'text',
    value = $bindable(''),
    placeholder = '',
    label,
    error,
    disabled = false,
    required = false,
    id,
    class: className = ''
  }: {
    type?: string;
    value?: string | number;
    placeholder?: string;
    label?: string;
    error?: string;
    disabled?: boolean;
    required?: boolean;
    id?: string;
    class?: string;
  } = $props();

  const inputId = id || `input-${Math.random().toString(36).substr(2, 9)}`;
</script>

<div class={`input-wrapper ${className}`}>
  {#if label}
    <label for={inputId} class="input-label">
      {label}
      {#if required}
        <span class="required">*</span>
      {/if}
    </label>
  {/if}

  <input
    {type}
    bind:value
    {placeholder}
    {disabled}
    {required}
    id={inputId}
    class="input"
    class:error
    class:disabled
  />

  {#if error}
    <p class="input-error">
      <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <circle cx="12" cy="12" r="10"/>
        <line x1="12" y1="8" x2="12" y2="12"/>
        <line x1="12" y1="16" x2="12.01" y2="16"/>
      </svg>
      {error}
    </p>
  {/if}
</div>

<style>
  .input-wrapper {
    width: 100%;
  }

  .input-label {
    display: block;
    font-size: 0.875rem;
    font-weight: 500;
    color: var(--foreground);
    margin-bottom: 0.5rem;
    letter-spacing: -0.01em;
  }

  .required {
    color: var(--destructive);
    margin-left: 0.125rem;
  }

  .input {
    width: 100%;
    padding: 0.75rem 1rem;
    font-size: 0.875rem;
    color: var(--foreground);
    background: var(--background);
    border: 1px solid var(--border);
    border-radius: var(--radius-md);
    transition: all 0.2s ease;
    font-family: var(--font-sans);
  }

  .input::placeholder {
    color: var(--muted-foreground);
  }

  .input:hover:not(.disabled) {
    border-color: var(--primary);
  }

  .input:focus {
    outline: none;
    border-color: var(--primary);
    box-shadow: 0 0 0 3px color-mix(in oklch, var(--primary), transparent 90%);
  }

  .input.error {
    border-color: var(--destructive);
  }

  .input.error:focus {
    box-shadow: 0 0 0 3px color-mix(in oklch, var(--destructive), transparent 90%);
  }

  .input.disabled {
    opacity: 0.5;
    cursor: not-allowed;
    background: var(--muted);
  }

  .input-error {
    display: flex;
    align-items: center;
    gap: 0.375rem;
    margin-top: 0.5rem;
    font-size: 0.813rem;
    color: var(--destructive);
  }

  .input-error svg {
    flex-shrink: 0;
  }

  /* Date input specific styles */
  input[type="date"]::-webkit-calendar-picker-indicator {
    cursor: pointer;
    opacity: 0.6;
    transition: opacity 0.2s ease;
  }

  input[type="date"]::-webkit-calendar-picker-indicator:hover {
    opacity: 1;
  }
</style>