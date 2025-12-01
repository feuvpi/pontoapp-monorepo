-- Script de inicialização do banco de dados PontoAPP
-- Executado automaticamente quando o container do PostgreSQL é criado

-- Garantir que o banco principal existe
SELECT 'CREATE DATABASE pontoapp_dev'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'pontoapp_dev')\gexec

-- Conectar ao banco
    \c pontoapp_dev

-- Criar extensões úteis
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- Para busca full-text

-- Criar schema public se não existir (geralmente já existe)
CREATE SCHEMA IF NOT EXISTS public;

-- Garantir permissões no schema public
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public;

-- Comentários
COMMENT ON DATABASE pontoapp_dev IS 'Database for PontoAPP - Time Tracking SaaS';
COMMENT ON SCHEMA public IS 'Schema for system-wide tables (Tenants, Subscriptions)';

-- Log
SELECT 'Database pontoapp_dev initialized successfully' AS status;