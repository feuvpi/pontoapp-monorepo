<script lang="ts">
	import type { User } from '$lib/types';
	import { formatDate } from '$lib/utils/format';
	import { USER_ROLE_LABELS } from '$lib/utils/constants';
	import { Edit, Trash2, CheckCircle, XCircle, Key, Clock } from 'lucide-svelte';
	import { createEventDispatcher } from 'svelte';

	interface Props {
		users: User[];
		isLoading?: boolean;
	}

	let { users, isLoading = false }: Props = $props();

	const dispatch = createEventDispatcher<{
		edit: User;
		deactivate: User;
		activate: User;
		resetPassword: User;
		viewTimeline: User;
	}>();
</script>

<div class="rounded-lg border border-border bg-card">
	<div class="overflow-x-auto">
		<table class="w-full">
			<thead class="border-b border-border bg-muted/50">
				<tr>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Nome</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">E-mail</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Matrícula</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Departamento</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Cargo</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Status</th>
					<th class="px-4 py-3 text-left text-sm font-medium text-foreground">Cadastro</th>
					<th class="px-4 py-3 text-right text-sm font-medium text-foreground">Ações</th>
				</tr>
			</thead>
			<tbody class="divide-y divide-border">
				{#if isLoading}
					<tr>
						<td colspan="8" class="px-4 py-8 text-center text-sm text-muted-foreground">
							Carregando...
						</td>
					</tr>
				{:else if users.length === 0}
					<tr>
						<td colspan="8" class="px-4 py-8 text-center text-sm text-muted-foreground">
							Nenhum funcionário encontrado
						</td>
					</tr>
				{:else}
					{#each users as user (user.id)}
						<tr class="hover:bg-muted/30 transition-colors">
							<td class="px-4 py-3 text-sm font-medium text-foreground">
								{user.fullName}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{user.email}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{user.employeeCode || '-'}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{user.department || '-'}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{USER_ROLE_LABELS[user.role]}
							</td>
							<td class="px-4 py-3">
								{#if user.isActive}
									<span
										class="inline-flex items-center gap-1 rounded-full bg-success-50 px-2 py-1 text-xs font-medium text-success-500"
									>
										<CheckCircle class="h-3 w-3" />
										Ativo
									</span>
								{:else}
									<span
										class="inline-flex items-center gap-1 rounded-full bg-muted px-2 py-1 text-xs font-medium text-muted-foreground"
									>
										<XCircle class="h-3 w-3" />
										Inativo
									</span>
								{/if}
							</td>
							<td class="px-4 py-3 text-sm text-muted-foreground">
								{formatDate(user.createdAt)}
							</td>
							<td class="px-4 py-3">
								<div class="flex items-center justify-end gap-2">
									<button
										onclick={() => dispatch('viewTimeline', user)}
										class="rounded p-1 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
										title="Ver registros de hoje"
									>
										<Clock class="h-4 w-4" />
									</button>

									<button
										onclick={() => dispatch('edit', user)}
										class="rounded p-1 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
										title="Editar"
									>
										<Edit class="h-4 w-4" />
									</button>

									<button
										onclick={() => dispatch('resetPassword', user)}
										class="rounded p-1 text-muted-foreground transition-colors hover:bg-accent hover:text-foreground"
										title="Resetar senha"
									>
										<Key class="h-4 w-4" />
									</button>

									{#if user.isActive}
										<button
											onclick={() => dispatch('deactivate', user)}
											class="rounded p-1 text-muted-foreground transition-colors hover:bg-destructive/10 hover:text-destructive"
											title="Desativar"
										>
											<Trash2 class="h-4 w-4" />
										</button>
									{:else}
										<button
											onclick={() => dispatch('activate', user)}
											class="rounded p-1 text-muted-foreground transition-colors hover:bg-success-50 hover:text-success-500"
											title="Reativar"
										>
											<CheckCircle class="h-4 w-4" />
										</button>
									{/if}
								</div>
							</td>
						</tr>
					{/each}
				{/if}
			</tbody>
		</table>
	</div>
</div>