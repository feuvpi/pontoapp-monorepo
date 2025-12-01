# SvelteKit Code Standards

## Estrutura do Projeto

- Componentes em `/src/lib/components`
- Stores em `/src/lib/stores`
- Utilitários em `/src/lib/utils`
- Rotas em `/src/routes`

## Convenções de Código

- Componentes em PascalCase
- Variáveis e funções em camelCase
- Arquivos em kebab-case
- Um componente por arquivo
- Usar TypeScript para tipos
- Prefixo `$:` para valores reativos derivados

## CSS/Estilo

- Usar TailwindCSS
- Classes utilitárias via diretiva `class:`
- Estilos globais em `/src/app.css`

## Estado da Aplicação

- Usar Svelte stores para estado global
- Props para estado de componente
- Context API para estado de árvore de componentes

## Requisições HTTP

- Usar fetch API com wrapper de serviço
- Tratamento de erros consistente
- Loading states para todas as requisições
