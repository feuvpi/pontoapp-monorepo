-- Função para criar um novo tenant (schema) dinamicamente
CREATE OR REPLACE FUNCTION create_tenant(tenant_name TEXT)
RETURNS VOID AS $$
DECLARE
    schema_name TEXT := 'tenant_' || tenant_name;
BEGIN
    -- Criar o schema
    EXECUTE 'CREATE SCHEMA IF NOT EXISTS ' || schema_name;
    
    -- Definir permissões padrão
    EXECUTE 'GRANT USAGE ON SCHEMA ' || schema_name || ' TO pontoapp';
    EXECUTE 'ALTER DEFAULT PRIVILEGES IN SCHEMA ' || schema_name || ' GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO pontoapp';
    EXECUTE 'ALTER DEFAULT PRIVILEGES IN SCHEMA ' || schema_name || ' GRANT USAGE, SELECT ON SEQUENCES TO pontoapp';
    
    -- Registrar o tenant na tabela principal
    INSERT INTO public.tenants(name, schema) VALUES (tenant_name, schema_name)
    ON CONFLICT (name) DO NOTHING;
END;
$$ LANGUAGE plpgsql;

-- Tabela para rastrear tenants
CREATE TABLE IF NOT EXISTS public.tenants (
    id SERIAL PRIMARY KEY,
    name TEXT UNIQUE NOT NULL,
    schema TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    active BOOLEAN DEFAULT TRUE
);

-- Criar tenant de exemplo
SELECT create_tenant('demo');
