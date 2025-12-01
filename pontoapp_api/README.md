# PontoAPP - Sistema de Registro de Ponto SaaS

Sistema multi-tenant de registro de ponto com suporte a biometria, desenvolvido em .NET 8 com arquitetura CQRS e PostgreSQL.

## 🚀 Primeiros Passos

### Pré-requisitos

- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL 16 (via Docker)

### 1. Iniciar o Banco de Dados

```bash
# Na raiz do projeto
docker-compose up -d

# Verificar se está rodando
docker ps
```

Serviços disponíveis:
- **PostgreSQL**: `localhost:5432`
- **PgAdmin**: `http://localhost:5050` (admin@pontoapp.com / admin)

### 2. Aplicar Migrations

```bash
# Migration do SystemDbContext (Tenants, Subscriptions)
cd PontoAPP.Infrastructure
dotnet ef migrations add InitialSystemMigration --context SystemDbContext --output-dir Data/Migrations/System --startup-project ../PontoAPP.API
dotnet ef database update --context SystemDbContext --startup-project ../PontoAPP.API

# Migration do TenantDbContext (Users, TimeRecords) - template
dotnet ef migrations add InitialTenantMigration --context TenantDbContext --output-dir Data/Migrations/Tenant --startup-project ../PontoAPP.API

# Voltar para raiz
cd ..
```

### 3. Rodar a API

```bash
cd PontoAPP.API
dotnet run
```

A API estará disponível em:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

## 📋 Estrutura do Projeto

```
PontoAPP/
├── PontoAPP.Domain/          # Entidades, Enums, Interfaces
├── PontoAPP.Application/     # Commands, Queries, DTOs, Validators
├── PontoAPP.Infrastructure/  # DbContexts, Repositories, Services
├── PontoAPP.API/            # Controllers, Middlewares, Configuration
└── docker-compose.yml       # PostgreSQL + PgAdmin
```

## 🏗️ Arquitetura Multi-Tenant

### Schema-based Separation

- **Schema `public`**: Dados globais (Tenants, Subscriptions)
- **Schema `tenant_{slug}`**: Dados isolados por tenant (Users, TimeRecords)

### Resolução de Tenant

O tenant é resolvido por:
1. Header `X-Tenant-Id` (GUID ou slug)
2. Subdomain (ex: `empresa.pontoapp.com`)
3. Query string `?tenantId=...`

## 🔐 Autenticação

### Fluxo de Registro

1. **POST /api/v1/tenants** - Cria tenant + schema + subscription trial
2. **POST /api/v1/auth/register** - Cria primeiro usuário (Admin)
3. **POST /api/v1/auth/login** - Autentica e retorna JWT

### JWT Token

Incluir no header das requisições:
```
Authorization: Bearer {token}
```

## 📊 Endpoints Principais

### Tenants
- `POST /api/v1/tenants` - Criar tenant
- `GET /api/v1/tenants/{id}` - Buscar tenant

### Autenticação
- `POST /api/v1/auth/register` - Registrar usuário
- `POST /api/v1/auth/login` - Login

### Usuários
- `GET /api/v1/users` - Listar usuários do tenant
- `POST /api/v1/users` - Criar usuário
- `PUT /api/v1/users/{id}` - Atualizar usuário

### Registros de Ponto
- `POST /api/v1/timerecords` - Registrar ponto
- `GET /api/v1/timerecords` - Listar registros
- `GET /api/v1/timerecords/user/{userId}` - Registros de um usuário

## 🧪 Testando Manualmente

### 1. Criar Tenant

```bash
POST http://localhost:5000/api/v1/tenants
Content-Type: application/json

{
  "name": "Empresa Teste",
  "slug": "empresa-teste",
  "email": "admin@empresa.com",
  "companyDocument": "12345678000190"
}
```

### 2. Registrar Usuário Admin

```bash
POST http://localhost:5000/api/v1/auth/register
Content-Type: application/json
X-Tenant-Id: empresa-teste

{
  "fullName": "Admin Teste",
  "email": "admin@empresa.com",
  "password": "Senha@123"
}
```

### 3. Login

```bash
POST http://localhost:5000/api/v1/auth/login
Content-Type: application/json
X-Tenant-Id: empresa-teste

{
  "email": "admin@empresa.com",
  "password": "Senha@123"
}
```

### 4. Registrar Ponto

```bash
POST http://localhost:5000/api/v1/timerecords
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant-Id: empresa-teste

{
  "type": "ClockIn",
  "authenticationType": "Biometric",
  "latitude": -23.550520,
  "longitude": -46.633308
}
```

## 🛠️ Comandos Úteis

### Entity Framework

```bash
# Adicionar migration
dotnet ef migrations add MigrationName --context ContextName --startup-project ../PontoAPP.API

# Aplicar migrations
dotnet ef database update --context ContextName --startup-project ../PontoAPP.API

# Remover última migration
dotnet ef migrations remove --context ContextName --startup-project ../PontoAPP.API

# Ver migrations aplicadas
dotnet ef migrations list --context ContextName --startup-project ../PontoAPP.API
```

### Docker

```bash
# Iniciar serviços
docker-compose up -d

# Ver logs
docker-compose logs -f postgres

# Parar serviços
docker-compose down

# Limpar volumes (CUIDADO: apaga dados)
docker-compose down -v
```

## 📝 Próximas Implementações

- [ ] Middlewares (TenantResolution, ExceptionHandling)
- [ ] Identity Services (PasswordHasher, JwtTokenService)
- [ ] Application Commands & Queries
- [ ] Validators (FluentValidation)
- [ ] Controllers completos
- [ ] Testes unitários e integração

## 🤝 Contribuindo

1. Clone o repositório
2. Crie uma branch: `git checkout -b feature/nova-feature`
3. Commit suas mudanças: `git commit -m 'Add nova feature'`
4. Push para a branch: `git push origin feature/nova-feature`
5. Abra um Pull Request

## 📄 Licença

[Definir licença]