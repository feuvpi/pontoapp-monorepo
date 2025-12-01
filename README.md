PontoApp
PontoApp é uma solução SaaS moderna para controle de ponto com foco em mobilidade, segurança e compliance.
Visão Geral
PontoApp permite que empresas de todos os tamanhos gerenciem registros de ponto de forma eficiente, utilizando os smartphones dos colaboradores como dispositivos de registro, eliminando a necessidade de equipamentos dedicados.
Principais Funcionalidades

Registro de ponto via smartphone com geolocalização
Verificação biométrica nativa do dispositivo (FaceID/TouchID/Fingerprint)
Validação por geolocalização com definição de perímetros autorizados
Sistema multi-tenant com isolamento total de dados
Relatórios detalhados de horas trabalhadas, ausências e conformidade
Exportação de dados para Excel e PDF
Gestão de equipes com diferentes níveis de acesso
Notificações automáticas para atrasos e horas extras
Modo terminal para dispositivos compartilhados com reconhecimento facial via OpenCV (em desenvolvimento)

Stack Tecnológica

Backend: .NET 8 API REST
Banco de Dados: PostgreSQL (multi-tenant via schemas)
Mobile App: Flutter (Android/iOS)
Web Admin (futuro): SvelteKit
Infraestrutura: Docker, Digital Ocean (ou similar)
CI/CD: GitHub Actions

Arquitetura
PontoApp utiliza uma arquitetura em camadas com separação clara de responsabilidades:

Core: Entidades, DTOs, Interfaces, Regras de Negócio
Infrastructure: Persistência, Implementações de Repositórios
Service: Lógica de negócio, Autenticação, Validações
API: Controllers, Middlewares, Configurações
Mobile: Interface de usuário, Lógica de apresentação

Multi-tenancy

Implementação via schemas PostgreSQL
Isolamento total de dados entre tenants
Middlewares para detecção e roteamento automático de tenants

Segurança

Autenticação JWT
Armazenamento seguro de templates biométricos (nunca dados brutos)
HTTPS para todas as comunicações
Validação de entrada com FluentValidation
Tratamento centralizado de exceções

Escalabilidade

Arquitetura preparada para escalar horizontalmente
Baixo consumo de recursos por tenant
Banco de dados otimizado para multi-tenancy
Possibilidade de migração para microsserviços no futuro

Implantação
PontoApp pode ser implantado facilmente em:

Digital Ocean Droplets
Azure App Service
AWS ECS/EKS
Google Cloud Run
Qualquer ambiente que suporte Docker

Estrutura do Monorepo
Copyponto-app/
├── api-dotnet/        # API principal em .NET 8
│   ├── API/           # Camada de apresentação (controllers)
│   ├── Core/          # Entidades, DTOs, Interfaces
│   ├── Infrastructure/# Persistência e implementações
│   ├── Service/       # Lógica de negócio
│   ├── Tests/         # Testes automatizados
├── mobile/            # Aplicativo Flutter
│   ├── lib/           # Código fonte do app
│   ├── test/          # Testes automatizados
├── web-admin/         # Painel administrativo (futuro)
├── docs/              # Documentação
├── infrastructure/    # Configurações de infraestrutura
Começando
Para iniciar o desenvolvimento:

Clone o repositório
Configure o banco de dados PostgreSQL
Execute a API .NET
Rode o aplicativo Flutter

Veja instruções detalhadas em cada diretório do projeto.
Roadmap & Backlog
Fase 1: MVP (2-3 meses)

 Arquitetura multi-tenant com PostgreSQL
 API .NET com autenticação e autorização
 Aplicativo mobile com registro de ponto básico
 Verificação biométrica via APIs nativas do dispositivo
 Validação por geolocalização
 Cadastro de empresas (tenants) e usuários
 Relatórios básicos de horas trabalhadas

Fase 2: Expansão (3-4 meses)

 Relatórios avançados e dashboards
 Exportação para Excel/PDF
 Notificações push para gestores e colaboradores
 Gestão de justificativas de ausência
 Banco de horas automatizado
 Definição de jornadas e escalas de trabalho
 Implementação inicial de reconhecimento facial com OpenCV

Fase 3: Integração (2-3 meses)

 Portal administrativo web em SvelteKit
 Integração com sistemas de folha de pagamento
 API para integrações externas
 Autenticação via SSO/SAML

Fase 4: Avançado (3-4 meses)

 Modo terminal para dispositivos compartilhados
 Algoritmos avançados de reconhecimento facial
 Detecção de fraudes e tentativas de burla
 Plataforma de marketplace para extensões
 Apps dedicados para gerentes e administradores