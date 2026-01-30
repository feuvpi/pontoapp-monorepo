<script lang="ts">
  import { reportsService } from '$lib/services/reports.service';
  import { usersService } from '$lib/services/users.service';
  import Button from '$lib/components/ui/button.svelte';
  import Input from '$lib/components/ui/input.svelte';
  import Select from '$lib/components/ui/select.svelte';
  import Card from '$lib/components/ui/card.svelte';
  import Tabs from '$lib/components/ui/tabs.svelte';
  import PageHeader from '$lib/components/layout/page-header.svelte';
  import LoadingSpinner from '$lib/components/shared/loading-spinner.svelte';
  import { toast } from 'svelte-sonner';
  import type { User } from '$lib/types';

  // States
  let users = $state<User[]>([]);
  let loadingUsers = $state(true);
  let generatingReport = $state(false);
  let previewUrl = $state<string | null>(null);
  let activeTab = $state('espelho');

  // Espelho de Ponto
  let espelhoUserId = $state('');
  let espelhoYear = $state(new Date().getFullYear());
  let espelhoMonth = $state(new Date().getMonth() + 1);

  // AFD
  let afdStartDate = $state('');
  let afdEndDate = $state('');
  let afdUserId = $state('');

  // ACJEF
  let acjefYear = $state(new Date().getFullYear());
  let acjefMonth = $state(new Date().getMonth() + 1);
  let acjefUserId = $state('');

  // Lifecycle
  $effect(() => {
    loadUsers();
  });

  async function loadUsers() {
    try {
      loadingUsers = true;
      users = await usersService.getAll({ activeOnly: true });
    } catch (error) {
      toast.error('Erro ao carregar funcionários');
      console.error(error);
    } finally {
      loadingUsers = false;
    }
  }

  // ========== ESPELHO DE PONTO ==========

  async function handleEspelhoPreview() {
    if (!espelhoUserId) {
      toast.error('Selecione um funcionário');
      return;
    }

    try {
      generatingReport = true;
      const blob = await reportsService.generateEspelho({
        userId: espelhoUserId,
        year: espelhoYear,
        month: espelhoMonth
      });

      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
      previewUrl = URL.createObjectURL(blob);
      toast.success('Relatório gerado com sucesso!');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Erro ao gerar relatório');
    } finally {
      generatingReport = false;
    }
  }

  async function handleEspelhoDownload() {
    if (!espelhoUserId) {
      toast.error('Selecione um funcionário');
      return;
    }

    try {
      generatingReport = true;
      const blob = await reportsService.generateEspelho({
        userId: espelhoUserId,
        year: espelhoYear,
        month: espelhoMonth
      });

      const user = users.find(u => u.id === espelhoUserId);
      const username = user?.fullName.replace(/\s+/g, '_') || 'funcionario';
      const filename = `Espelho_Ponto_${username}_${espelhoYear}_${espelhoMonth.toString().padStart(2, '0')}.pdf`;
      
      reportsService.downloadBlob(blob, filename);
      toast.success('Download iniciado!');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Erro ao baixar relatório');
    } finally {
      generatingReport = false;
    }
  }

  // ========== AFD ==========

  async function handleAFDDownload() {
    if (!afdStartDate || !afdEndDate) {
      toast.error('Preencha as datas');
      return;
    }

    const start = new Date(afdStartDate);
    const end = new Date(afdEndDate);

    if (start > end) {
      toast.error('Data inicial não pode ser maior que data final');
      return;
    }

    const diffDays = (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24);
    if (diffDays > 366) {
      toast.error('Período máximo é de 1 ano');
      return;
    }

    try {
      generatingReport = true;
      const blob = await reportsService.generateAFD({
        startDate: afdStartDate,
        endDate: afdEndDate,
        userId: afdUserId || undefined
      });

      const filename = `AFD_${afdStartDate.replace(/-/g, '')}_${afdEndDate.replace(/-/g, '')}.txt`;
      reportsService.downloadBlob(blob, filename);
      toast.success('AFD gerado com sucesso!');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Erro ao gerar AFD');
    } finally {
      generatingReport = false;
    }
  }

  // ========== ACJEF ==========

  async function handleACJEFDownload() {
    try {
      generatingReport = true;
      const data = await reportsService.generateACJEF({
        year: acjefYear,
        month: acjefMonth,
        userId: acjefUserId || undefined
      });

      const json = JSON.stringify(data, null, 2);
      const blob = new Blob([json], { type: 'application/json' });
      const filename = `ACJEF_${acjefYear}_${acjefMonth.toString().padStart(2, '0')}.json`;
      reportsService.downloadBlob(blob, filename);
      toast.success('ACJEF gerado com sucesso!');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Erro ao gerar ACJEF');
    } finally {
      generatingReport = false;
    }
  }

  // Cleanup
  $effect(() => {
    return () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    };
  });

  const tabs = [
    { 
      id: 'espelho', 
      label: 'Espelho de Ponto',
      icon: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>'
    },
    { 
      id: 'afd', 
      label: 'AFD',
      icon: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14 2 14 8 20 8"/></svg>'
    },
    { 
      id: 'acjef', 
      label: 'ACJEF',
      icon: '<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M13 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V9z"/><polyline points="13 2 13 9 20 9"/><path d="M8 13h2"/><path d="M8 17h2"/><path d="M14 13h2"/><path d="M14 17h2"/></svg>'
    }
  ];

  const userOptions = $derived(
    users.map(u => ({
      value: u.id,
      label: u.fullName,
      subtitle: u.employeeCode || undefined
    }))
  );

  const months = [
    { value: 1, label: 'Janeiro' },
    { value: 2, label: 'Fevereiro' },
    { value: 3, label: 'Março' },
    { value: 4, label: 'Abril' },
    { value: 5, label: 'Maio' },
    { value: 6, label: 'Junho' },
    { value: 7, label: 'Julho' },
    { value: 8, label: 'Agosto' },
    { value: 9, label: 'Setembro' },
    { value: 10, label: 'Outubro' },
    { value: 11, label: 'Novembro' },
    { value: 12, label: 'Dezembro' }
  ];

  const currentYear = new Date().getFullYear();
  const years = Array.from({ length: 5 }, (_, i) => ({
    value: currentYear - i,
    label: (currentYear - i).toString()
  }));
</script>

<PageHeader
  title="Relatórios"
  description="Gere e visualize relatórios de ponto e documentos fiscais"
/>

<div class="reports-layout">
  <!-- Controls Panel -->
  <div class="controls-panel">
    <Tabs {tabs} bind:activeTab />

    <div class="controls-content">
      {#if activeTab === 'espelho'}
        <Card variant="glass" class="control-card">
          <div class="card-header">
            <h3 class="font-display">Espelho de Ponto</h3>
            <p class="card-description">Relatório mensal individual em PDF</p>
          </div>

          {#if loadingUsers}
            <LoadingSpinner />
          {:else}
            <div class="form-fields">
              <Select
                label="Funcionário"
                options={[{ value: '', label: 'Selecione um funcionário' }, ...userOptions]}
                bind:value={espelhoUserId}
                placeholder="Selecione o funcionário"
              />

              <div class="form-row">
                <Select
                  label="Ano"
                  options={years}
                  bind:value={espelhoYear}
                />
                <Select
                  label="Mês"
                  options={months}
                  bind:value={espelhoMonth}
                />
              </div>

              <div class="action-buttons">
                <Button
                  variant="outline"
                  onclick={handleEspelhoPreview}
                  disabled={generatingReport || !espelhoUserId}
                  class="flex-1"
                >
                  {#if generatingReport}
                    <LoadingSpinner size="sm" />
                  {:else}
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                      <circle cx="12" cy="12" r="3"/>
                    </svg>
                  {/if}
                  <span>Visualizar</span>
                </Button>
                <Button
                  onclick={handleEspelhoDownload}
                  disabled={generatingReport || !espelhoUserId}
                  class="flex-1"
                >
                  {#if generatingReport}
                    <LoadingSpinner size="sm" />
                  {:else}
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                      <polyline points="7 10 12 15 17 10"/>
                      <line x1="12" y1="15" x2="12" y2="3"/>
                    </svg>
                  {/if}
                  <span>Baixar PDF</span>
                </Button>
              </div>
            </div>
          {/if}
        </Card>

      {:else if activeTab === 'afd'}
        <Card variant="glass" class="control-card">
          <div class="card-header">
            <h3 class="font-display">AFD - Arquivo Fonte de Dados</h3>
            <p class="card-description">Layout 9 - Portaria 671/2021</p>
          </div>

          <div class="form-fields">
            <Select
              label="Funcionário (opcional)"
              options={[{ value: '', label: 'Todos os funcionários' }, ...userOptions]}
              bind:value={afdUserId}
            />

            <div class="form-row">
              <Input
                label="Data Inicial"
                type="date"
                bind:value={afdStartDate}
              />
              <Input
                label="Data Final"
                type="date"
                bind:value={afdEndDate}
              />
            </div>

            <Button
              onclick={handleAFDDownload}
              disabled={generatingReport || !afdStartDate || !afdEndDate}
              class="w-full"
            >
              {#if generatingReport}
                <LoadingSpinner size="sm" />
              {:else}
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                  <polyline points="7 10 12 15 17 10"/>
                  <line x1="12" y1="15" x2="12" y2="3"/>
                </svg>
              {/if}
              <span>Baixar AFD (.txt)</span>
            </Button>
          </div>
        </Card>

      {:else if activeTab === 'acjef'}
        <Card variant="glass" class="control-card">
          <div class="card-header">
            <h3 class="font-display">ACJEF</h3>
            <p class="card-description">Arquivo Eletrônico de Jornada</p>
          </div>

          <div class="form-fields">
            <Select
              label="Funcionário (opcional)"
              options={[{ value: '', label: 'Todos os funcionários' }, ...userOptions]}
              bind:value={acjefUserId}
            />

            <div class="form-row">
              <Select
                label="Ano"
                options={years}
                bind:value={acjefYear}
              />
              <Select
                label="Mês"
                options={months}
                bind:value={acjefMonth}
              />
            </div>

            <Button
              onclick={handleACJEFDownload}
              disabled={generatingReport}
              class="w-full"
            >
              {#if generatingReport}
                <LoadingSpinner size="sm" />
              {:else}
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                  <polyline points="7 10 12 15 17 10"/>
                  <line x1="12" y1="15" x2="12" y2="3"/>
                </svg>
              {/if}
              <span>Baixar ACJEF (.json)</span>
            </Button>
          </div>
        </Card>
      {/if}
    </div>
  </div>

  <!-- Preview Panel -->
  <div class="preview-panel">
    <Card variant="glass" class="preview-card">
      <div class="preview-header">
        <h3 class="font-display">Pré-visualização</h3>
        <p class="preview-description">
          {activeTab === 'espelho' ? 'O PDF será exibido aqui após gerar' : 'Preview disponível apenas para Espelho de Ponto'}
        </p>
      </div>

      <div class="preview-content">
        {#if previewUrl && activeTab === 'espelho'}
          <iframe
            src={previewUrl}
            title="Preview do Relatório"
            class="preview-iframe"
          />
        {:else}
          <div class="preview-empty">
            <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
              <polyline points="14 2 14 8 20 8"/>
            </svg>
            <p class="empty-title font-display">Nenhum relatório gerado</p>
            <p class="empty-subtitle">
              {activeTab === 'espelho' 
                ? 'Selecione as opções e clique em "Visualizar"'
                : 'Arquivos AFD e ACJEF são baixados diretamente'
              }
            </p>
          </div>
        {/if}
      </div>
    </Card>
  </div>
</div>

<style>
  .reports-layout {
    display: grid;
    grid-template-columns: 400px 1fr;
    gap: 2rem;
    min-height: calc(100vh - 200px);
  }

  @media (max-width: 1280px) {
    .reports-layout {
      grid-template-columns: 1fr;
    }

    .preview-panel {
      order: -1;
    }
  }

  .controls-panel {
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .controls-content {
    flex: 1;
  }

  .control-card {
    height: 100%;
  }

  .card-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--border);
  }

  .card-header h3 {
    font-size: 1.25rem;
    font-weight: 700;
    color: var(--foreground);
    margin-bottom: 0.25rem;
  }

  .card-description {
    font-size: 0.875rem;
    color: var(--muted-foreground);
  }

  .form-fields {
    display: flex;
    flex-direction: column;
    gap: 1.25rem;
  }

  .form-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
  }

  .action-buttons {
    display: flex;
    gap: 0.75rem;
    margin-top: 0.5rem;
  }

  .preview-panel {
    position: sticky;
    top: 2rem;
    height: fit-content;
  }

  .preview-card {
    height: 100%;
    min-height: 700px;
  }

  .preview-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--border);
  }

  .preview-header h3 {
    font-size: 1.25rem;
    font-weight: 700;
    color: var(--foreground);
    margin-bottom: 0.25rem;
  }

  .preview-description {
    font-size: 0.875rem;
    color: var(--muted-foreground);
  }

  .preview-content {
    height: calc(100% - 100px);
    min-height: 600px;
  }

  .preview-iframe {
    width: 100%;
    height: 100%;
    border: 1px solid var(--border);
    border-radius: var(--radius-lg);
  }

  .preview-empty {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100%;
    text-align: center;
    padding: 2rem;
    border: 2px dashed var(--border);
    border-radius: var(--radius-lg);
  }

  .preview-empty svg {
    color: var(--muted-foreground);
    margin-bottom: 1rem;
  }

  .empty-title {
    font-size: 1.125rem;
    font-weight: 600;
    color: var(--foreground);
    margin-bottom: 0.5rem;
  }

  .empty-subtitle {
    font-size: 0.875rem;
    color: var(--muted-foreground);
    max-width: 300px;
  }
</style>